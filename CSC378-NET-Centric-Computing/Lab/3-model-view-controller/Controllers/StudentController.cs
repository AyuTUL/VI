using _3_model_view_controller.Models;
using Microsoft.AspNetCore.Mvc;

namespace _3_model_view_controller.Controllers
{
    public class StudentController: Controller
    {
        public IActionResult Index()
        {
            Student s = new Student()
            {
                Name = "Ram",
                Age = 21
            };
            return View(s);
        }
    }
}
