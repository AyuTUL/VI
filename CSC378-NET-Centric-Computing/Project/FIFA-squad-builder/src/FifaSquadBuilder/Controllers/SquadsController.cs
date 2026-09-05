using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using FifaSquadBuilder.Services.Calculations;
using FifaSquadBuilder.Services.Player;
using FifaSquadBuilder.Services.Squad;
using FifaSquadBuilder.ViewModels.Squad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Controllers;

[Authorize]
public class SquadsController : Controller
{
    private readonly ISquadService _squadService;
    private readonly ISquadStatisticsService _statisticsService;
    private readonly IWeakPositionService _weakPositionService;
    private readonly IPlayerSearchService _playerSearchService;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SquadsController(
        ISquadService squadService,
        ISquadStatisticsService statisticsService,
        IWeakPositionService weakPositionService,
        IPlayerSearchService playerSearchService,
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _squadService = squadService;
        _statisticsService = statisticsService;
        _weakPositionService = weakPositionService;
        _playerSearchService = playerSearchService;
        _db = db;
        _userManager = userManager;
    }

    private string CurrentUserId => _userManager.GetUserId(User)
        ?? throw new InvalidOperationException("Authenticated request with no user id - should be unreachable under [Authorize].");

    // GET /Squads
    public async Task<IActionResult> Index()
    {
        var squads = await _squadService.GetUserSquadsAsync(CurrentUserId);
        return View(squads);
    }

    // GET /Squads/Create
    public async Task<IActionResult> Create()
    {
        var vm = new SquadCreateViewModel { FormationOptions = await GetFormationOptionsAsync() };
        return View(vm);
    }

    // POST /Squads/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SquadCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.FormationOptions = await GetFormationOptionsAsync();
            return View(vm);
        }

        var id = await _squadService.CreateSquadAsync(CurrentUserId, vm.Name, vm.FormationId, vm.WageBudgetEUR);
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET /Squads/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var editor = await _squadService.GetSquadForEditAsync(id, CurrentUserId);
        if (editor == null) return NotFound(); // covers both "doesn't exist" and "not yours" identically

        var stats = await _statisticsService.ComputeAsync(id) ?? new SquadStatistics();
        var weak = await _weakPositionService.FindWeakPositionsAsync(id);

        var vm = new SquadDetailsViewModel
        {
            Editor = editor,
            Statistics = stats,
            WeakPositions = weak,
            FormationOptions = await GetFormationOptionsAsync(editor.FormationId),
            PositionOptions = await _db.Positions.OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Code} - {p.Name}" })
                .ToListAsync(),
        };
        return View(vm);
    }

    // POST /Squads/Rename/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rename(int id, string name)
    {
        var result = await _squadService.RenameSquadAsync(id, CurrentUserId, name);
        TempData["StatusMessage"] = result.Success ? "Squad renamed." : result.ErrorMessage;
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Squads/SetBudget/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetBudget(int id, decimal budget)
    {
        var result = await _squadService.SetWageBudgetAsync(id, CurrentUserId, budget);
        TempData["StatusMessage"] = result.Success ? "Budget updated." : result.ErrorMessage;
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Squads/ChangeFormation/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeFormation(int id, int formationId)
    {
        var result = await _squadService.ChangeFormationAsync(id, CurrentUserId, formationId);
        TempData["StatusMessage"] = result.Success
            ? "Formation changed - all players moved to the bench, reassign your starting XI."
            : result.ErrorMessage;
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Squads/AssignPlayer - AJAX, returns JSON
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPlayer(int squadId, int playerId, int? formationPositionId)
    {
        var result = await _squadService.AssignPlayerAsync(squadId, CurrentUserId, playerId, formationPositionId);
        return Json(new { success = result.Success, error = result.ErrorMessage });
    }

    // POST /Squads/RemovePlayer - AJAX, returns JSON
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePlayer(int squadId, int squadPlayerId)
    {
        var result = await _squadService.RemovePlayerAsync(squadId, CurrentUserId, squadPlayerId);
        return Json(new { success = result.Success, error = result.ErrorMessage });
    }

    // GET /Squads/PlayerSearch - AJAX, returns JSON. Not squad-scoped: browsing the
    // player database doesn't touch any squad's data, so no ownership check applies.
    [HttpGet]
    public async Task<IActionResult> PlayerSearch([FromQuery] PlayerSearchQuery query)
    {
        var results = await _playerSearchService.SearchAsync(query);
        return Json(results);
    }

    // GET /Squads/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var editor = await _squadService.GetSquadForEditAsync(id, CurrentUserId);
        if (editor == null) return NotFound();
        return View(editor);
    }

    // POST /Squads/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _squadService.DeleteSquadAsync(id, CurrentUserId);
        TempData["StatusMessage"] = result.Success ? "Squad deleted." : result.ErrorMessage;
        return RedirectToAction(nameof(Index));
    }

    private Task<List<SelectListItem>> GetFormationOptionsAsync(int? selectedFormationId = null) =>
        _db.Formations.OrderBy(f => f.Name)
            .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Name, Selected = selectedFormationId == f.Id })
            .ToListAsync();
}
