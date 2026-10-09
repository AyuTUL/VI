using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _5_GET_and_POST_endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employees = new[]
            {
                new { Id = 7, Name = "Son", Department = "LW" },
                new { Id = 9, Name = "Kane", Department = "ST" }
            };

            return Ok(employees);
        }

        [HttpPost]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            return Ok(employee);
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Department { get; set; } = "";
    }
}