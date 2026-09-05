using FifaSquadBuilder.Data;
using FifaSquadBuilder.Services.Player;
using FifaSquadBuilder.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class PlayersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IPlayerSearchService _searchService;

    public PlayersController(ApplicationDbContext db, IPlayerSearchService searchService)
    {
        _db = db;
        _searchService = searchService;
    }

    // GET /Admin/Players
    public async Task<IActionResult> Index([FromQuery] PlayerSearchQuery query)
    {
        var vm = new PlayerListViewModel
        {
            Query = query,
            Results = await _searchService.SearchAsync(query),
            PositionOptions = await GetPositionOptionsAsync(),
            ClubOptions = await GetClubOptionsAsync(),
            LeagueOptions = await GetLeagueOptionsAsync(),
            NationOptions = await GetNationOptionsAsync(),
        };
        return View(vm);
    }

    // GET /Admin/Players/Create
    public async Task<IActionResult> Create()
    {
        var vm = new PlayerFormViewModel
        {
            NationOptions = await GetNationOptionsAsync(),
            ClubOptions = await GetClubOptionsAsync(includeNone: true),
            PositionOptions = await GetPositionOptionsAsync(),
        };
        return View(vm);
    }

    // POST /Admin/Players/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlayerFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await RepopulateOptionsAsync(vm);
            return View(vm);
        }

        // A manually admin-created player has no dataset ID. Using a negative,
        // strictly-decreasing sequence keeps it out of the CSV's ID space (all
        // positive) so a future re-import can never collide with it.
        var minSourceId = await _db.Players.Select(p => (int?)p.SourceId).MinAsync() ?? 0;
        var newSourceId = Math.Min(minSourceId, 0) - 1;

        var player = new Models.Player
        {
            SourceId = newSourceId,
            CardImageUrl = vm.CardImageUrl,
            Name = vm.Name,
            Age = vm.Age,
            NationId = vm.NationId,
            ClubId = vm.ClubId,
            PositionId = vm.PositionId,
            Overall = vm.Overall,
            Potential = vm.Potential,
            Pace = vm.Pace,
            Shooting = vm.Shooting,
            Passing = vm.Passing,
            Dribbling = vm.Dribbling,
            Defending = vm.Defending,
            Physicality = vm.Physicality,
            ValueEUR = vm.ValueEUR,
            WageEUR = vm.WageEUR,
        };

        _db.Players.Add(player);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = $"Player '{player.Name}' created.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Admin/Players/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var player = await _db.Players.FindAsync(id);
        if (player == null) return NotFound();

        var vm = new PlayerFormViewModel
        {
            Id = player.Id,
            Name = player.Name,
            CardImageUrl = player.CardImageUrl,
            Age = player.Age,
            NationId = player.NationId,
            ClubId = player.ClubId,
            PositionId = player.PositionId,
            Overall = player.Overall,
            Potential = player.Potential,
            Pace = player.Pace,
            Shooting = player.Shooting,
            Passing = player.Passing,
            Dribbling = player.Dribbling,
            Defending = player.Defending,
            Physicality = player.Physicality,
            ValueEUR = player.ValueEUR,
            WageEUR = player.WageEUR,
            NationOptions = await GetNationOptionsAsync(),
            ClubOptions = await GetClubOptionsAsync(includeNone: true),
            PositionOptions = await GetPositionOptionsAsync(),
        };
        return View(vm);
    }

    // POST /Admin/Players/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlayerFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await RepopulateOptionsAsync(vm);
            return View(vm);
        }

        var player = await _db.Players.FindAsync(id);
        if (player == null) return NotFound();

        player.Name = vm.Name;
        player.CardImageUrl = vm.CardImageUrl;
        player.Age = vm.Age;
        player.NationId = vm.NationId;
        player.ClubId = vm.ClubId;
        player.PositionId = vm.PositionId;
        player.Overall = vm.Overall;
        player.Potential = vm.Potential;
        player.Pace = vm.Pace;
        player.Shooting = vm.Shooting;
        player.Passing = vm.Passing;
        player.Dribbling = vm.Dribbling;
        player.Defending = vm.Defending;
        player.Physicality = vm.Physicality;
        player.ValueEUR = vm.ValueEUR;
        player.WageEUR = vm.WageEUR;

        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = $"Player '{player.Name}' updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Admin/Players/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var player = await _db.Players
            .Include(p => p.Nation)
            .Include(p => p.Club)
            .Include(p => p.Position)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (player == null) return NotFound();

        var squadCount = await _db.SquadPlayers.CountAsync(sp => sp.PlayerId == id);
        ViewBag.SquadUsageCount = squadCount;

        return View(player);
    }

    // POST /Admin/Players/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var player = await _db.Players.FindAsync(id);
        if (player == null) return NotFound();

        _db.Players.Remove(player);
        try
        {
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = $"Player '{player.Name}' deleted.";
        }
        catch (DbUpdateException)
        {
            // FK is Restrict on purpose (see ApplicationDbContext) - a player still
            // used in someone's saved squad cannot be silently deleted out from under
            // them. Friendly message, no stack trace, per Rule 24.
            TempData["StatusMessage"] =
                $"Cannot delete '{player.Name}' - it is still used in one or more saved squads. " +
                "Remove it from those squads first.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task RepopulateOptionsAsync(PlayerFormViewModel vm)
    {
        vm.NationOptions = await GetNationOptionsAsync();
        vm.ClubOptions = await GetClubOptionsAsync(includeNone: true);
        vm.PositionOptions = await GetPositionOptionsAsync();
    }

    private Task<List<SelectListItem>> GetPositionOptionsAsync() =>
        _db.Positions.OrderBy(p => p.Name)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Code} - {p.Name}" })
            .ToListAsync();

    private async Task<List<SelectListItem>> GetClubOptionsAsync(bool includeNone = false)
    {
        var options = await _db.Clubs.OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();
        if (includeNone)
        {
            options.Insert(0, new SelectListItem { Value = "", Text = "(No club / free agent)" });
        }
        return options;
    }

    private Task<List<SelectListItem>> GetLeagueOptionsAsync() =>
        _db.Leagues.OrderBy(l => l.Name)
            .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Name })
            .ToListAsync();

    private Task<List<SelectListItem>> GetNationOptionsAsync() =>
        _db.Nations.OrderBy(n => n.Name)
            .Select(n => new SelectListItem { Value = n.Id.ToString(), Text = n.Name })
            .ToListAsync();
}
