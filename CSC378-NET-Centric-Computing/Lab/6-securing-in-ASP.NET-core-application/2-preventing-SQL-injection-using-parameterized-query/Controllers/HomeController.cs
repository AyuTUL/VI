using _2_preventing_SQL_injection_using_parameterized_query.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _2_preventing_SQL_injection_using_parameterized_query.Controllers
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
