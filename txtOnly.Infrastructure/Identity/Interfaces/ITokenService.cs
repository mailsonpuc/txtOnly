
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using txtOnly.Infrastructure.Identity.Models;


namespace txtOnly.Infrastructure.Identity.Interfaces;

public interface ITokenService
{
    Task<TokenDTO> CreateTokenAsync(txtOnly.Infrastructure.Identity.ApplicationUser user);
    Task<TokenDTO?> RefreshTokenAsync(string refreshToken);
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration configuration);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration);
}
