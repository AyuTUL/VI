using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using FifaSquadBuilder.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FifaSquadBuilder.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : Controller
{
    private const string AdminRoleName = "Admin";

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET /Admin/Users
    public async Task<IActionResult> Index()
    {
        var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == AdminRoleName);
        var adminUserIds = adminRole == null
            ? new HashSet<string>()
            : (await _db.UserRoles.Where(ur => ur.RoleId == adminRole.Id).Select(ur => ur.UserId).ToListAsync()).ToHashSet();

        var users = await _db.Users.OrderBy(u => u.Email).ToListAsync();

        var list = users.Select(u => new AdminUserListItem
        {
            Id = u.Id,
            Email = u.Email ?? u.UserName ?? u.Id,
            IsAdmin = adminUserIds.Contains(u.Id),
        }).ToList();

        ViewBag.CurrentUserId = _userManager.GetUserId(User);
        return View(list);
    }

    // POST /Admin/Users/ToggleAdmin/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string id)
    {
        var currentUserId = _userManager.GetUserId(User);
        if (id == currentUserId)
        {
            // Prevents an admin from locking themselves out by removing their own access.
            // If another admin needs to demote this account, they can do it from theirs.
            TempData["StatusMessage"] = "You cannot change your own admin status.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var isAdmin = await _userManager.IsInRoleAsync(user, AdminRoleName);
        if (isAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, AdminRoleName);
            TempData["StatusMessage"] = $"{user.Email} is no longer an Admin.";
        }
        else
        {
            await _userManager.AddToRoleAsync(user, AdminRoleName);
            TempData["StatusMessage"] = $"{user.Email} is now an Admin.";
        }

        return RedirectToAction(nameof(Index));
    }
}
