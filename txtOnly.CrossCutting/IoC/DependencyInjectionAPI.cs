

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Services;
using txtOnly.Domain.Interfaces;
using txtOnly.Infrastructure.Context;
using txtOnly.Infrastructure.HealthChecks;
using txtOnly.Infrastructure.Identity;
using txtOnly.Infrastructure.Identity.Interfaces;
using txtOnly.Infrastructure.Identity.Services;
using txtOnly.Infrastructure.Repositories;
using IdentityApplicationUser = txtOnly.Infrastructure.Identity.ApplicationUser;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionAPI
{
    public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
    {
        //Usando SQL Server
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                // String de conexão vinda do appsettings.json
                configuration.GetConnectionString("DefaultConnection"),

                // Define onde ficarão as migrations
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        services.AddIdentity<IdentityApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var secretKey = configuration["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
            throw new InvalidOperationException("Jwt:SecretKey must contain at least 32 characters.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddAuthorization();
        services.AddScoped<ITokenService, TokenService>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new UserService(unitOfWork, unitOfWork.UserRepository);
        });
        services.AddScoped<IPostService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new PostService(unitOfWork, unitOfWork.PostRepository, unitOfWork.UserRepository);
        });
        services.AddScoped<ICommentService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new CommentService(unitOfWork, unitOfWork.CommentRepository,
                unitOfWork.UserRepository, unitOfWork.PostRepository);
        });
        services.AddScoped<ILikeService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new LikeService(unitOfWork, unitOfWork.LikeRepository,
                unitOfWork.UserRepository, unitOfWork.PostRepository);
        });
        services.AddScoped<IFollowService>(serviceProvider =>
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new FollowService(unitOfWork, unitOfWork.FollowRepository, unitOfWork.UserRepository);
        });

        return services;
    }

}
