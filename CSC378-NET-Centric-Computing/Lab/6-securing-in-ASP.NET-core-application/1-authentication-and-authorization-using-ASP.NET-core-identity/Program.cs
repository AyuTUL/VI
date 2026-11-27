using _1_authentication_and_authorization_using_ASP.NET_core_identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var name in new[] { "User", "Admin" })
    {
        if (!await roles.RoleExistsAsync(name))
        {
            var result = await roles.CreateAsync(new IdentityRole(name));
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }

    var email = app.Configuration["SeedAdmin:Email"];
    var password = app.Configuration["SeedAdmin:Password"];
    if (string.IsNullOrWhiteSpace(email) != string.IsNullOrWhiteSpace(password))
        throw new InvalidOperationException("Set both SeedAdmin:Email and SeedAdmin:Password.");

    if (!string.IsNullOrWhiteSpace(email))
    {
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var admin = await users.FindByNameAsync(email);
        if (admin is null)
        {
            admin = new IdentityUser { UserName = email, Email = email };
            var result = await users.CreateAsync(admin, password!);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        if (!await users.IsInRoleAsync(admin, "Admin"))
        {
            var result = await users.AddToRoleAsync(admin, "Admin");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute("default", "{controller=Account}/{action=Register}/{id?}").WithStaticAssets();
app.Run();
