

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using txtOnly.Infrastructure.Identity.Interfaces;
using txtOnly.Infrastructure.Identity.Services;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionJWT
{
	public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		// ===============================
		// AUTENTICAÇÃO JWT
		// ===============================
		var secretKey = configuration["Jwt:SecretKey"];
		if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
			throw new InvalidOperationException("Jwt:SecretKey must contain at least 32 characters.");

		var issuer = configuration["Jwt:Issuer"];
		var audience = configuration["Jwt:Audience"];
		if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
			throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience are required.");

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = issuer,
				ValidateAudience = true,
				ValidAudience = audience,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
				ValidateLifetime = true,
				NameClaimType = ClaimTypes.Name,
				RoleClaimType = ClaimTypes.Role,
				ClockSkew = TimeSpan.Zero
			};

			options.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
					logger.LogError(context.Exception, "Authentication failed.");
					return Task.CompletedTask;
				},
				OnTokenValidated = context =>
				{
					var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
					logger.LogInformation("Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
					return Task.CompletedTask;
				},
				OnChallenge = context =>
				{
					var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
					logger.LogWarning("Challenge triggered for path: {Path}, error: {Error}", context.Request.Path, context.Error);
					return Task.CompletedTask;
				}
			};
		});

		services.AddAuthorization();
		services.AddScoped<ITokenService, TokenService>();

		return services;
	}

}
