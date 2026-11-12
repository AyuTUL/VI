using Microsoft.AspNetCore.Mvc;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
