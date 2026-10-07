using System.ComponentModel.DataAnnotations;

namespace _3_client_and_server_side_model_validation.Models
{
    public class Employee
    {
        [Required(ErrorMessage = "Employee name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(10000, 500000, ErrorMessage = "Salary must be between 10,000 and 500,000.")]
        public decimal Salary { get; set; }
    }
}