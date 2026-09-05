using FifaSquadBuilder.Data;
using FifaSquadBuilder.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class FormationsController : Controller
{
    private readonly ApplicationDbContext _db;

    public FormationsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Admin/Formations
    public async Task<IActionResult> Index()
    {
        var formations = await _db.Formations
            .Select(f => new FormationListItem { Id = f.Id, Name = f.Name, SlotCount = f.Positions.Count })
            .OrderBy(f => f.Name)
            .ToListAsync();
        return View(formations);
    }

    // GET /Admin/Formations/Create
    public IActionResult Create() => View(new FormationFormViewModel());

    // POST /Admin/Formations/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FormationFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await _db.Formations.AnyAsync(f => f.Name == vm.Name))
        {
            ModelState.AddModelError(nameof(vm.Name), "A formation with this name already exists.");
            return View(vm);
        }

        var formation = new Models.Formation { Name = vm.Name };
        _db.Formations.Add(formation);
        await _db.SaveChangesAsync();

        TempData["StatusMessage"] = $"Formation '{formation.Name}' created. Now add its 11 pitch positions.";
        return RedirectToAction(nameof(Edit), new { id = formation.Id });
    }

    // GET /Admin/Formations/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var formation = await _db.Formations
            .Include(f => f.Positions).ThenInclude(fp => fp.Position)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (formation == null) return NotFound();

        var vm = new FormationEditViewModel
        {
            Formation = new FormationFormViewModel { Id = formation.Id, Name = formation.Name },
            Positions = formation.Positions
                .OrderBy(p => p.OrderIndex)
                .Select(p => new FormationPositionRow
                {
                    Id = p.Id,
                    PositionCode = p.Position.Code,
                    PositionId = p.PositionId,
                    X = p.X,
                    Y = p.Y,
                    OrderIndex = p.OrderIndex,
                })
                .ToList(),
            NewPosition = new FormationPositionFormViewModel { FormationId = id, OrderIndex = formation.Positions.Count },
            PositionOptions = await GetPositionOptionsAsync(),
        };
        return View(vm);
    }

    // POST /Admin/Formations/Rename/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rename(int id, string name)
    {
        var formation = await _db.Formations.FindAsync(id);
        if (formation == null) return NotFound();

        if (string.IsNullOrWhiteSpace(name) || name.Length > 20)
        {
            TempData["StatusMessage"] = "Formation name must be 1-20 characters.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        formation.Name = name.Trim();
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "Formation renamed.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    // POST /Admin/Formations/AddPosition
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPosition(FormationPositionFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["StatusMessage"] = "Could not add position - check the values entered.";
            return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
        }

        var orderTaken = await _db.FormationPositions
            .AnyAsync(fp => fp.FormationId == vm.FormationId && fp.OrderIndex == vm.OrderIndex);
        if (orderTaken)
        {
            TempData["StatusMessage"] = $"Order index {vm.OrderIndex} is already used in this formation.";
            return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
        }

        var currentCount = await _db.FormationPositions.CountAsync(fp => fp.FormationId == vm.FormationId);
        if (currentCount >= 11)
        {
            TempData["StatusMessage"] = "This formation already has 11 positions defined (a full starting XI).";
            return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
        }

        _db.FormationPositions.Add(new Models.FormationPosition
        {
            FormationId = vm.FormationId,
            PositionId = vm.PositionId,
            X = vm.X,
            Y = vm.Y,
            OrderIndex = vm.OrderIndex,
        });
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "Position added.";
        return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
    }

    // POST /Admin/Formations/EditPosition
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPosition(FormationPositionFormViewModel vm)
    {
        var fp = await _db.FormationPositions.FindAsync(vm.Id);
        if (fp == null) return NotFound();

        if (!ModelState.IsValid)
        {
            TempData["StatusMessage"] = "Could not update position - check the values entered.";
            return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
        }

        var orderTaken = await _db.FormationPositions
            .AnyAsync(other => other.FormationId == vm.FormationId && other.OrderIndex == vm.OrderIndex && other.Id != vm.Id);
        if (orderTaken)
        {
            TempData["StatusMessage"] = $"Order index {vm.OrderIndex} is already used by another slot in this formation.";
            return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
        }

        fp.PositionId = vm.PositionId;
        fp.X = vm.X;
        fp.Y = vm.Y;
        fp.OrderIndex = vm.OrderIndex;
        await _db.SaveChangesAsync();
        TempData["StatusMessage"] = "Position updated.";
        return RedirectToAction(nameof(Edit), new { id = vm.FormationId });
    }

    // POST /Admin/Formations/RemovePosition/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePosition(int id, int formationId)
    {
        var fp = await _db.FormationPositions.FindAsync(id);
        if (fp == null) return NotFound();

        _db.FormationPositions.Remove(fp);
        try
        {
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "Position removed.";
        }
        catch (DbUpdateException)
        {
            TempData["StatusMessage"] =
                "Cannot remove this slot - a saved squad currently has a player assigned to it.";
        }
        return RedirectToAction(nameof(Edit), new { id = formationId });
    }

    // GET /Admin/Formations/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var formation = await _db.Formations.FindAsync(id);
        if (formation == null) return NotFound();

        ViewBag.SquadUsageCount = await _db.Squads.CountAsync(s => s.FormationId == id);
        return View(formation);
    }

    // POST /Admin/Formations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var formation = await _db.Formations.FindAsync(id);
        if (formation == null) return NotFound();

        _db.Formations.Remove(formation);
        try
        {
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = $"Formation '{formation.Name}' deleted.";
        }
        catch (DbUpdateException)
        {
            TempData["StatusMessage"] =
                $"Cannot delete '{formation.Name}' - it is used by one or more saved squads. Reassign those squads to a different formation first.";
        }
        return RedirectToAction(nameof(Index));
    }

    private Task<List<SelectListItem>> GetPositionOptionsAsync() =>
        _db.Positions.OrderBy(p => p.Name)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Code} - {p.Name}" })
            .ToListAsync();
}
