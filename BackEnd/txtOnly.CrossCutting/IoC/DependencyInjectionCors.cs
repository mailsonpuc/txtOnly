using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionCors
{
   
    public static IServiceCollection AddInfrastructureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:5173",                   // front Vite dev server
                        "http://localhost:5079",                   // back HTTP
                        "https://localhost:7014",                  // back HTTPS
                        "https://pet-shoop-full-stack.vercel.app", // front produção
                        "http://127.0.0.1",
                        "http://127.0.0.1:80",                    //nginx deploy front
                        "http://0.0.0.0" ,                         // Nginx via IP local (Porta 80) 
                        "http://localhost:5174"                   // front Vite dev server (Porta 5174)
                
                    )
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization");
            });
        });


        return services;
    }

}