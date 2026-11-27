using System.ComponentModel.DataAnnotations;

namespace _1_authentication_and_authorization_using_ASP.NET_core_identity.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
