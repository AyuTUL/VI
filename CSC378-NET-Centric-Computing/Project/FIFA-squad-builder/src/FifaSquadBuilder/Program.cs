using FifaSquadBuilder.Data;
using FifaSquadBuilder.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// --- Database ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Reasonable defaults for an academic project; not weakened arbitrarily.
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Not calling .AddDefaultUI(): we use our own AccountController + views (Controllers/AccountController.cs)
// instead of Identity's scaffolded Razor Pages, to get exact /Account/Login, /Account/Register routes
// per the spec's page list. Still 100% Identity underneath - UserManager/SignInManager, no custom hashing.

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// --- Application services (registered here as they're implemented in later phases) ---
builder.Services.AddScoped<FifaSquadBuilder.Services.Player.IPlayerImportService, FifaSquadBuilder.Services.Player.PlayerImportService>();
builder.Services.AddScoped<FifaSquadBuilder.Services.Player.IPlayerSearchService, FifaSquadBuilder.Services.Player.PlayerSearchService>();
builder.Services.AddScoped<FifaSquadBuilder.Services.Squad.ISquadService, FifaSquadBuilder.Services.Squad.SquadService>();
builder.Services.AddScoped<FifaSquadBuilder.Services.Calculations.IChemistryCalculator, FifaSquadBuilder.Services.Calculations.ChemistryCalculator>();
builder.Services.AddScoped<FifaSquadBuilder.Services.Calculations.ISquadStatisticsService, FifaSquadBuilder.Services.Calculations.SquadStatisticsService>();
builder.Services.AddScoped<FifaSquadBuilder.Services.Calculations.IWeakPositionService, FifaSquadBuilder.Services.Calculations.WeakPositionService>();

// --- MVC ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // required by Identity's default UI

var app = builder.Build();

// --- Bootstrap admin account (idempotent) ---
// Registration (AccountController) never grants Admin - it's only ever assigned here,
// from configuration, or later by an existing Admin via user management (Phase 9).
// Reads from configuration so the real password is never hardcoded here; set it via
// the AdminBootstrap:Email / AdminBootstrap:Password keys, ideally as environment
// variables (AdminBootstrap__Email / AdminBootstrap__Password) rather than committed
// to appsettings.json. If unset, this block does nothing - no accidental default admin.
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var adminEmail = config["AdminBootstrap:Email"];
    var adminPassword = config["AdminBootstrap:Password"];

    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                // Defensive fallback only - the migration's HasData seed should already
                // have created this. If it hasn't run yet, don't crash startup over it.
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Bootstrap admin account created: {Email}", adminEmail);
                }
                else
                {
                    logger.LogWarning("Bootstrap admin account could not be created: {Errors}",
                        string.Join("; ", createResult.Errors.Select(e => e.Description)));
                }
            }
            else if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(existingAdmin, "Admin");
            }
        }
        catch (Exception ex)
        {
            // Most likely cause: migrations haven't been applied yet, so the Identity
            // tables don't exist. Log clearly and let startup continue rather than
            // crashing the whole app over a seeding step - the developer running this
            // for the first time needs "run migrations" pointed out, not a raw stack trace.
            logger.LogWarning(ex, "Skipped admin bootstrap - has 'dotnet ef database update' been run yet?");
        }
    }
}

// --- HTTP pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Players}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
