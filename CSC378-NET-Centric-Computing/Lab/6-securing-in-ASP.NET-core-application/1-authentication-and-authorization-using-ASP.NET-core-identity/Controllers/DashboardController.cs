using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Controllers;

public class DashboardController : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Admin() => View();
}
