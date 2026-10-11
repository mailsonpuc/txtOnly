

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using txtOnly.Infrastructure.Identity.Interfaces;
using txtOnly.Infrastructure.Identity.Models;

namespace txtOnly.Infrastructure.Identity.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<txtOnly.Infrastructure.Identity.ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public TokenService(UserManager<txtOnly.Infrastructure.Identity.ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<TokenDTO> CreateTokenAsync(txtOnly.Infrastructure.Identity.ApplicationUser user)
    {
        return await CreateAndPersistTokenAsync(user)
            ?? throw new InvalidOperationException("Unable to persist token state.");
    }

    private async Task<TokenDTO?> CreateAndPersistTokenAsync(txtOnly.Infrastructure.Identity.ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? user.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = GenerateAccessToken(claims, _configuration);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = GetRefreshTokenExpiration();
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return null;

        return new TokenDTO
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken,
            Expiration = token.ValidTo
        };
    }

    public async Task<TokenDTO?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(item => item.RefreshToken == refreshToken);
        if (user is null || user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return await CreateAndPersistTokenAsync(user);
    }

    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SecretKey must contain at least 32 characters.");
        }

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expires = DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpireMinutes", 60));
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expires,
            signingCredentials: credentials);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SecretKey must contain at least 32 characters.");
        }

        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private DateTime GetRefreshTokenExpiration()
    {
        var validity = _configuration.GetValue("Jwt:RefreshTokenValidityInMinutes", 60);
        if (validity <= 0)
        {
            validity = 60;
        }

        return DateTime.UtcNow.AddMinutes(validity);
    }

}
