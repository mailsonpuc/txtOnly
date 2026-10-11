

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionCors
{
    public static IServiceCollection AddInfrastructureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                    .WithHeaders("Content-Type", "Authorization")
                    .WithExposedHeaders("X-Pagination", "Server");
            });
        });

        return services;
    }
}
