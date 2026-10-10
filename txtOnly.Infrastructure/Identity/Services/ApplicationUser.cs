
using Microsoft.AspNetCore.Identity;

namespace txtOnly.Infrastructure.Identity.Services;

public class ApplicationUser: IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

}
