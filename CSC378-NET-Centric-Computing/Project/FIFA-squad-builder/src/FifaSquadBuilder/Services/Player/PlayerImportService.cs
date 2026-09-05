using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FifaSquadBuilder.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Services.Player;

public class PlayerImportService : IPlayerImportService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _environment;

    private const string FreeAgentClubName = "Free agent";
    private const int BatchSize = 500;

    public PlayerImportService(ApplicationDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public async Task<PlayerImportResult> ImportAsync(string playersCsvPath, string teamsCsvPath)
    {
        if (!File.Exists(playersCsvPath))
        {
            throw new FileNotFoundException(
                $"Players CSV not found at '{playersCsvPath}'. Expected columns (from FIFA23 dataset): " +
                "ID, Name, Age, Nationality, Overall, Potential, BestPosition, Club, ValueEUR, WageEUR.",
                playersCsvPath);
        }
        if (!File.Exists(teamsCsvPath))
        {
            throw new FileNotFoundException(
                $"Teams CSV not found at '{teamsCsvPath}'. Expected columns: Name, League, LeagueId. " +
                "This file is required because the players CSV has no league column - league is only " +
                "resolvable by joining on club name.",
                teamsCsvPath);
        }

        var result = new PlayerImportResult();
        var cardImageBySourceId = BuildLocalCardImageIndex();

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
        };

        // --- Step 1: preload existing lookups so we upsert instead of duplicate ---
        var positionByCode = await _db.Positions.ToDictionaryAsync(p => p.Code, p => p, StringComparer.OrdinalIgnoreCase);
        var nationByName = await _db.Nations.ToDictionaryAsync(n => n.Name, n => n, StringComparer.OrdinalIgnoreCase);
        var leagueBySourceId = await _db.Leagues
            .Where(l => l.SourceLeagueId != null)
            .ToDictionaryAsync(l => l.SourceLeagueId!.Value, l => l);
        var clubByName = await _db.Clubs.ToDictionaryAsync(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

        // --- Step 2: teams_fifa23.csv -> resolve League + Club rows first ---
        // clubName -> (sourceLeagueId, leagueDisplayName), read once into memory since the
        // file is small (~680 rows) and every player row needs to look a club up in it.
        var clubToLeague = new Dictionary<string, (int SourceLeagueId, string LeagueName)>(StringComparer.OrdinalIgnoreCase);

        using (var teamsReader = new StreamReader(teamsCsvPath))
        using (var teamsCsv = new CsvReader(teamsReader, csvConfig))
        {
            await teamsCsv.ReadAsync();
            teamsCsv.ReadHeader();

            while (await teamsCsv.ReadAsync())
            {
                var clubName = teamsCsv.GetField("Name")?.Trim();
                var leagueName = teamsCsv.GetField("League")?.Trim();
                var leagueIdRaw = teamsCsv.GetField("LeagueId");

                if (string.IsNullOrWhiteSpace(clubName) || string.IsNullOrWhiteSpace(leagueName)
                    || !int.TryParse(leagueIdRaw, out var sourceLeagueId))
                {
                    result.Errors.Add($"teams_fifa23.csv: skipped malformed row (Name='{clubName}')");
                    continue;
                }

                clubToLeague[clubName] = (sourceLeagueId, leagueName);

                if (!leagueBySourceId.TryGetValue(sourceLeagueId, out var league))
                {
                    league = new Models.League { SourceLeagueId = sourceLeagueId, Name = leagueName };
                    _db.Leagues.Add(league);
                    leagueBySourceId[sourceLeagueId] = league;
                    result.LeaguesCreated++;
                }

                if (!clubByName.TryGetValue(clubName, out var club))
                {
                    club = new Models.Club { Name = clubName, League = league };
                    _db.Clubs.Add(club);
                    clubByName[clubName] = club;
                    result.ClubsCreated++;
                }
                else
                {
                    // Keep club's league in sync if it changed since last import.
                    club.League = league;
                }
            }
        }

        await _db.SaveChangesAsync(); // flush leagues/clubs so their Ids exist before player FK fixup

        // --- Step 3: players_fifa23.csv ---
        using (var playersReader = new StreamReader(playersCsvPath))
        using (var playersCsv = new CsvReader(playersReader, csvConfig))
        {
            await playersCsv.ReadAsync();
            playersCsv.ReadHeader();

            var seenSourceIds = new HashSet<int>(); // de-dupes the dataset's ~119 exact-duplicate rows
            var pendingCount = 0;
            var rowNumber = 1; // header is row 1

            var existingPlayers = await _db.Players.ToDictionaryAsync(p => p.SourceId, p => p);

            while (await playersCsv.ReadAsync())
            {
                rowNumber++;

                var idRaw = playersCsv.GetField("ID");
                var name = playersCsv.GetField("Name")?.Trim();
                var ageRaw = playersCsv.GetField("Age");
                var nationality = playersCsv.GetField("Nationality")?.Trim();
                var overallRaw = playersCsv.GetField("Overall");
                var potentialRaw = playersCsv.GetField("Potential");
                var bestPosition = playersCsv.GetField("BestPosition")?.Trim();
                var clubName = playersCsv.GetField("Club")?.Trim();
                var valueRaw = playersCsv.GetField("ValueEUR");
                var wageRaw = playersCsv.GetField("WageEUR");
                var pace = ReadOptionalInt(playersCsv, "PaceTotal");
                var shooting = ReadOptionalInt(playersCsv, "ShootingTotal");
                var passing = ReadOptionalInt(playersCsv, "PassingTotal");
                var dribbling = ReadOptionalInt(playersCsv, "DribblingTotal");
                var defending = ReadOptionalInt(playersCsv, "DefendingTotal");
                var physicality = ReadOptionalInt(playersCsv, "PhysicalityTotal");
                string? cardImageUrl = null;

                if (!int.TryParse(idRaw, out var sourceId)
                    || string.IsNullOrWhiteSpace(name)
                    || !int.TryParse(ageRaw, out var age)
                    || string.IsNullOrWhiteSpace(nationality)
                    || !int.TryParse(overallRaw, out var overall)
                    || !int.TryParse(potentialRaw, out var potential)
                    || string.IsNullOrWhiteSpace(bestPosition)
                    || !decimal.TryParse(valueRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                    || !decimal.TryParse(wageRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var wage))
                {
                    result.Errors.Add($"players_fifa23.csv row {rowNumber}: missing/invalid required field(s), skipped.");
                    result.RowsSkipped++;
                    continue;
                }

                if (!seenSourceIds.Add(sourceId))
                {
                    continue; // exact-duplicate ID row already processed - silently skip, not an error
                }

                if (cardImageBySourceId.TryGetValue(sourceId, out var localCardImageUrl))
                {
                    cardImageUrl = localCardImageUrl;
                    result.CardImagesAssigned++;
                }

                if (!positionByCode.TryGetValue(bestPosition, out var position))
                {
                    result.Errors.Add($"players_fifa23.csv row {rowNumber}: unknown BestPosition '{bestPosition}' for player '{name}', skipped.");
                    result.RowsSkipped++;
                    continue;
                }

                if (!nationByName.TryGetValue(nationality, out var nation))
                {
                    nation = new Models.Nation { Name = nationality, Code = null };
                    _db.Nations.Add(nation);
                    nationByName[nationality] = nation;
                    result.NationsCreated++;
                }

                Models.Club? club = null;
                if (!string.IsNullOrWhiteSpace(clubName) && !clubName.Equals(FreeAgentClubName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!clubByName.TryGetValue(clubName, out club))
                    {
                        result.Errors.Add($"players_fifa23.csv row {rowNumber}: club '{clubName}' for player '{name}' not found in teams_fifa23.csv - imported with no club.");
                    }
                }

                if (existingPlayers.TryGetValue(sourceId, out var existing))
                {
                    existing.Name = name;
                    existing.Age = age;
                    existing.Nation = nation;
                    existing.Club = club;
                    existing.Position = position;
                    existing.Overall = overall;
                    existing.Potential = potential;
                    existing.Pace = pace;
                    existing.Shooting = shooting;
                    existing.Passing = passing;
                    existing.Dribbling = dribbling;
                    existing.Defending = defending;
                    existing.Physicality = physicality;
                    existing.ValueEUR = value;
                    existing.WageEUR = wage;
                    if (!string.IsNullOrWhiteSpace(cardImageUrl))
                    {
                        existing.CardImageUrl = cardImageUrl;
                    }
                    result.PlayersUpdated++;
                }
                else
                {
                    var player = new Models.Player
                    {
                        SourceId = sourceId,
                        Name = name,
                        Age = age,
                        Nation = nation,
                        Club = club,
                        Position = position,
                        Overall = overall,
                        Potential = potential,
                        Pace = pace,
                        Shooting = shooting,
                        Passing = passing,
                        Dribbling = dribbling,
                        Defending = defending,
                        Physicality = physicality,
                        ValueEUR = value,
                        WageEUR = wage,
                        CardImageUrl = cardImageUrl,
                    };
                    _db.Players.Add(player);
                    existingPlayers[sourceId] = player;
                    result.PlayersCreated++;
                }

                pendingCount++;
                if (pendingCount >= BatchSize)
                {
                    await _db.SaveChangesAsync();
                    pendingCount = 0;
                }
            }

            if (pendingCount > 0)
            {
                await _db.SaveChangesAsync();
            }
        }

        return result;
    }

    private static int? ReadOptionalInt(CsvReader csv, string fieldName)
    {
        var raw = csv.GetField(fieldName);
        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    private Dictionary<int, string> BuildLocalCardImageIndex()
    {
        var webRootPath = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var cardRoot = Path.Combine(webRootPath, "player-cards");
        if (!Directory.Exists(cardRoot))
        {
            return new Dictionary<int, string>();
        }

        return Directory.EnumerateFiles(cardRoot, "*.png", SearchOption.AllDirectories)
            .Select(path => new LocalCardImage(path, TryReadSourceId(Path.GetFileName(path)), GetCardImagePriority(path)))
            .Where(image => image.SourceId != null)
            .GroupBy(image => image.SourceId!.Value)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var best = group
                        .OrderBy(image => image.Priority)
                        .ThenBy(image => image.Path, StringComparer.OrdinalIgnoreCase)
                        .First();

                    return "/" + Path.GetRelativePath(webRootPath, best.Path)
                        .Replace(Path.DirectorySeparatorChar, '/')
                        .Replace(Path.AltDirectorySeparatorChar, '/');
                });
    }

    private static int? TryReadSourceId(string fileName)
    {
        var dashIndex = fileName.IndexOf('-');
        var idText = dashIndex > 0
            ? fileName[..dashIndex]
            : Path.GetFileNameWithoutExtension(fileName);

        return int.TryParse(idText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sourceId)
            ? sourceId
            : null;
    }

    private static int GetCardImagePriority(string path)
    {
        var normalized = path.Replace(Path.DirectorySeparatorChar, '/').ToLowerInvariant();

        if (normalized.Contains("/player-cards/ea/fc25/"))
        {
            return 0;
        }
        if (normalized.Contains("/player-cards/ea/fc24/"))
        {
            return 1;
        }
        if (normalized.Contains("/player-cards/ea/fc26/"))
        {
            return 2;
        }
        if (normalized.Contains("/player-cards/ea/"))
        {
            return 3;
        }

        return 4;
    }

    private sealed record LocalCardImage(string Path, int? SourceId, int Priority);
}
