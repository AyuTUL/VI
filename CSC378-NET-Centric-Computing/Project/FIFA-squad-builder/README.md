# FIFA Squad Builder

A FIFA Ultimate Team-style squad builder: pick real players from a FIFA23 dataset,
build a starting XI on a formation-driven pitch, and see chemistry, ratings, value,
and wage-budget statistics update as you go.

This is an intermediate-level academic/personal project. It does not reproduce EA's
proprietary chemistry or rating algorithms, and it does not use any EA/FIFA artwork,
logos, or card designs - the visuals are original CSS/SVG.

## Features

- ASP.NET Core Identity auth (register/login/logout), two roles: `User`, `Admin`.
- Admin CRUD for Players and Formations (including per-slot pitch coordinates).
- CSV import/upsert of the FIFA23 player dataset into MySQL, safe to re-run.
- Multiple squads per user, ownership enforced server-side on every action.
- Formation-driven pitch editor: click a slot, search/filter players, assign.
- Bench (7 slots), duplicate-player prevention (DB-enforced, not just client-side).
- Live squad statistics: Starting XI rating, squad rating, average age, average
  potential, squad value, wage budget vs. actual with over-budget warning, chemistry.
- Weak-position finder: compares each starter against the best unused alternative
  at their position.

## Technology stack

- ASP.NET Core MVC (.NET 8 LTS), C#
- Entity Framework Core 8 + Pomelo.EntityFrameworkCore.MySql
- MySQL 8
- ASP.NET Core Identity
- Razor views, vanilla JavaScript, hand-written CSS (no UI framework beyond Bootstrap's grid/utility classes for basic layout)
- xUnit + EF Core InMemory provider for service-layer tests
- CsvHelper for dataset import

## Requirements

- .NET 8 SDK
- MySQL Server 8.x, running locally or reachable over the network
- The FIFA23 dataset CSVs (`players_fifa23.csv`, `teams_fifa23.csv`) - included under
  `src/FifaSquadBuilder/App_Data/dataset/`

## Installation

```bash
git clone <this repo>
cd FifaSquadBuilder
dotnet restore
```

## Database setup

1. Create a MySQL user/database, or let EF Core create the database on first migration.
2. Set your connection string. **Do not commit real credentials** - `appsettings.json`
   ships with placeholders (`user=CHANGE_ME;password=CHANGE_ME`). Options:
   - Edit `appsettings.Development.json` locally (already gitignored-in-spirit -
     placeholder values only are committed), or
   - Set the `ConnectionStrings__DefaultConnection` environment variable, which
     overrides the JSON value (standard ASP.NET Core config precedence).

## Migrations

```bash
cd src/FifaSquadBuilder
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This creates all Identity tables plus the domain schema, and seeds: 15 positions,
7 formations (77 pitch-slot definitions total), and the `Admin`/`User` roles.

> Note: at the time this project was built, `dotnet restore` could not reach
> `nuget.org` in the development sandbox used, so migrations could not be generated
> or executed there. Everything above is the correct, standard command sequence -
> run it in an environment with normal internet access.

## Dataset import

The players/teams CSVs are already in `App_Data/dataset/`. After migrations:

1. Register an account, then promote it to Admin (see below), or configure the
   bootstrap admin (see below) and log in as that account.
2. Go to **Admin → Players → Run dataset import**, or navigate directly to
   `/Admin/Import` and click **Run Import**.

The importer is idempotent - re-running it updates existing rows (matched by the
dataset's own `ID` column) rather than duplicating them.

### What the importer actually does, based on inspecting the real files

- `players_fifa23.csv` has no `League` column. League is only derivable by joining
  each player's `Club` name against `teams_fifa23.csv`'s `Name`/`League`/`LeagueId`
  columns - the importer does this join in memory before touching player rows.
- The literal club value `"Free agent"` (not a blank string) means "no club" in this
  dataset - handled explicitly, not treated as a bad row.
- `BestPosition` values map 1:1 onto this project's 15 seeded position codes.
- `ValueEUR`/`WageEUR` are plain integers in the source file - no currency-symbol
  parsing needed.
- ~119 rows are exact full-row duplicate IDs in the source file; the importer keeps
  the first occurrence and skips the rest silently (not treated as an error).
- `Nation.Code` (e.g. a 3-letter abbreviation) is left `null` by the importer. The
  dataset only provides full country names, not ISO codes, and guessing an
  abbreviation from the name is wrong often enough (e.g. "United States" is not
  "UNI") that it isn't done. An admin can fill it in by hand if it matters.

## Admin setup

Self-registration only ever grants the `User` role - this is deliberate, to close
the obvious privilege-escalation hole of a signup form choosing its own role.
Two ways to get an Admin account:

1. **Bootstrap on startup** (recommended for first run): set
   `AdminBootstrap:Email` / `AdminBootstrap:Password` in configuration (ideally as
   the environment variables `AdminBootstrap__Email` / `AdminBootstrap__Password`,
   not committed to `appsettings.json`). On the next app start, this account is
   created (or promoted to Admin if it already exists) idempotently. Leave these
   blank and nothing happens - no accidental default admin account.
2. **Promote via Admin → Users**: once you have any Admin account, go to `/Admin/Users`
   and click "Make Admin" next to any other account. An admin cannot change their own
   role from this screen (prevents accidental self-lockout) - a second admin, or the
   bootstrap step above, is needed for that.

## Running the project

```bash
cd src/FifaSquadBuilder
dotnet run
```

Then open the URL shown in the console (typically `https://localhost:5001` or similar).

## Testing

```bash
cd tests/FifaSquadBuilder.Tests
dotnet test
```

Covers: chemistry calculation (positional fit, club/league/nation link thresholds,
the 3-point-per-player cap), squad service rules (ownership isolation, duplicate-player
prevention via move-not-duplicate semantics, bench capacity, formation-change
non-destructiveness), and weak-position logic (correct gap detection, excluding
players already in the squad, respecting the significance threshold, not crashing
when no alternative exists). Uses EF Core's InMemory provider so tests don't need a
real MySQL server.

> As with migrations, `dotnet test` could not actually be executed in the sandbox
> this was built in (same nuget-restore blocker). The test code was written against
> the real service signatures and reviewed carefully, but has not been confirmed to
> pass - run it yourself as the first thing you do after cloning.

## Project architecture

```
FifaSquadBuilder/
├── Controllers/           Account, Squads (thin - delegate to Services)
├── Areas/Admin/            Admin-only Players, Formations, Import controllers
├── Data/                   ApplicationDbContext, SeedData (positions, formations, roles)
├── Models/                 EF entities
├── ViewModels/              Per-page view models - views never bind directly to entities
├── Services/
│   ├── Player/              Import, search/filter/pagination
│   ├── Squad/                Squad CRUD, player assignment, ownership enforcement
│   └── Calculations/         Chemistry, squad statistics, weak positions
├── Views/, Areas/Admin/Views/
└── wwwroot/css, wwwroot/js   squad-builder.css, squad-editor.js
```

Business logic lives in `Services/`, not controllers or Razor views. Every squad
operation takes the requesting user's id and enforces ownership as part of the same
database query that loads the squad (rather than "load, then check") - a squad
belonging to someone else and a squad that doesn't exist look identical to the
caller, so a manipulated URL id can't be used to probe for other users' data.

## Chemistry formula

**This app's own model - not a reproduction of EA's proprietary FIFA/UT chemistry
algorithm.** Only starting XI players are considered; the bench contributes nothing.

Per starting player, up to 3 points (each condition worth +1, total capped at 3):

- **Positional fit**: the player's natural position matches the pitch slot they're
  assigned to.
- **Club link**: at least 2 *other* starters share this player's club.
- **League link**: at least 3 *other* starters share this player's league.
- **Nation link**: at least 2 *other* starters share this player's nation.

Squad chemistry = sum of every starter's points, scaled onto a 0-100 display range
(max possible is 11 x 3 = 33 points, so the raw sum is multiplied by 100/33 and rounded).

## Squad-rating formula

Also this app's own simple, transparent formula - not EA's.

- **Starting XI Rating** = average `Overall` of the 11 starters (rounded).
- **Squad Rating** = average `Overall` across the whole squad, starting XI + bench.
  Deliberately a different number from Starting XI Rating (per FIFA23 field, `Overall`
  is the dataset's own rating).
- **Average Age** / **Average Potential** = averaged over the starting XI only
  (the team as it would actually take the pitch). `Potential` is used exactly as it
  appears in the dataset - never recalculated.
- **Squad Value** / **Total Wages** = summed over the whole squad (starting XI + bench).

## Weak-position algorithm

For each **filled** starting-XI slot:

1. Take the formation slot's required position and the assigned player's `Overall`.
2. Query the full player database for the highest-`Overall` player at that exact
   position, excluding every player already anywhere in this squad (starting XI or
   bench) - the tool won't recommend a player you already have.
3. If `(best available Overall) - (current player's Overall) >= 5`, report it as a
   weak position with the exact gap.
4. If no alternative exists at all for that position, the slot is silently skipped
   (not an error, not a false "weak" flag).

Empty slots aren't evaluated - "no player assigned" is a different UI concern than
"the assigned player is significantly worse than an alternative."

## Known limitations

- No drag-and-drop; player assignment is click-slot → search-modal → select, per the
  original scope decision to skip drag-and-drop for the first implementation.
- Bench capacity (7) and the implied 18-player squad cap are enforced in the service
  layer, not by a MySQL CHECK constraint (MySQL has no native "row count per group"
  constraint mechanism) - `SquadService` is the single source of truth for this rule.
- Changing a squad's formation moves every assigned player to the bench rather than
  trying to intelligently remap them to equivalent slots in the new formation - this
  is a deliberate non-destructive choice (nobody gets silently dropped from the squad)
  rather than an oversight.
- `Nation.Code` is unpopulated by the importer (see Dataset Import section above).
- This build's `dotnet restore` could not reach `nuget.org` in the sandbox it was
  developed in, so **no build, migration, or test run in this repository has actually
  been executed against real compiled assemblies or a real MySQL instance.** The code
  was written carefully against real, verified inputs (the actual dataset CSVs were
  inspected directly, not guessed) and reviewed for structural/logical correctness,
  but "compiles clean and runs correctly" has not been mechanically confirmed. Treat
  first-run `dotnet restore && dotnet build && dotnet test` as a required step, not
  a formality.
