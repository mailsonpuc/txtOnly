
using System.ComponentModel.DataAnnotations;


namespace txtOnly.Infrastructure.Identity.Models;

public class LoginDTO
{
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
