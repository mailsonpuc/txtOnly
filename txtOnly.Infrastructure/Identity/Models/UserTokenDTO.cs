
namespace txtOnly.Infrastructure.Identity.Models;

public class UserTokenDTO
{
    public string? Token { get; set; }
    public DateTime Expiration { get; set; }
}
