
using System.ComponentModel.DataAnnotations;

namespace txtOnly.Infrastructure.Identity.Models;

public class RegisterDTO
{
    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; set; }


    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    public string? Email { get; set; }


    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
