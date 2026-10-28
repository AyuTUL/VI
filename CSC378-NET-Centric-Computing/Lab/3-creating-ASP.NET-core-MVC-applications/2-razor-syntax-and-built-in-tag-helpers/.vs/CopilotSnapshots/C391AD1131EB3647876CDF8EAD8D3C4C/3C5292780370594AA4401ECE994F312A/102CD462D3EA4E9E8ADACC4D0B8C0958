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

        [HttpPost]
        public IActionResult Register(Registration model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            return View("Index", model);
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
