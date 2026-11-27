using _1_authentication_and_authorization_using_ASP.NET_core_identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Controllers;

public class AccountController(
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signIn) : Controller
{
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await users.CreateAsync(user, model.Password);
        if (result.Succeeded)
            result = await users.AddToRoleAsync(user, "User");

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        await signIn.SignInAsync(user, isPersistent: false);
        return RedirectToAction(nameof(Register));
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
            return View(model);

        if (await signIn.PasswordSignInAsync(model.Email, model.Password, false, false) is { Succeeded: true })
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(Register));

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction(nameof(Register));
    }

    public IActionResult AccessDenied() => View();
}
