using _1_authentication_and_authorization_using_ASP.NET_core_identity.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
