using _2_razor_syntax_and_built_in_tag_helpers.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _2_razor_syntax_and_built_in_tag_helpers.Controllers
{
    public class RegistrationController : Controller
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