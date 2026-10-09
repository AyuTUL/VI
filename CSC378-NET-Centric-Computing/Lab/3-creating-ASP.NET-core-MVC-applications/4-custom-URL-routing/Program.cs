
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "employee",
    pattern: "employee/details/{id}",
    defaults: new
    {
        controller = "Employee",
        action = "Details"
    });

app.Run();