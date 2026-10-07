using _3_client_and_server_side_model_validation.Models;
using Microsoft.AspNetCore.Mvc;

namespace _3_client_and_server_side_model_validation.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Employee employee)
        {
            if (ModelState.IsValid)
                return Content("Employee registered successfully!");
            return View(employee);
        }
    }
}