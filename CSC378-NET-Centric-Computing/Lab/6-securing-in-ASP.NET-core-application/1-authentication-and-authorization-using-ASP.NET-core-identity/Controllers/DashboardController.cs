using Microsoft.AspNetCore.Mvc;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }
        [Authorize(Roles = "Admin,User")]
        public IActionResult UserPanel()
        {
            return View();
        }
    }
}
