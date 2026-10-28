using Microsoft.AspNetCore.Mvc;

namespace _4_custom_URL_routing.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Details(int? id)
        {
            ViewBag.EmployeeId = id?.ToString() ?? "Not provided";
            return View();
        }
    }
}
