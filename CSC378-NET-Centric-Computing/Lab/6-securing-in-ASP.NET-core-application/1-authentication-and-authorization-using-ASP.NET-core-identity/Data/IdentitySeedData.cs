using Microsoft.AspNetCore.Identity;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Data;

public static class IdentitySeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var result = await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        const string email = "admin@example.com";
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Could not create demo admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Admin");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Could not assign Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
