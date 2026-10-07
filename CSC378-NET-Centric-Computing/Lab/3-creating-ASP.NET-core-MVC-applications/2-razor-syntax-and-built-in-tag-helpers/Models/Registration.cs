using System.ComponentModel.DataAnnotations;
namespace _2_razor_syntax_and_built_in_tag_helpers.Models
{
    public class Registration
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}