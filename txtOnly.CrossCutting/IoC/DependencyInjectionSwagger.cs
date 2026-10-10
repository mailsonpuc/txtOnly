using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionSwagger
{
    public static IServiceCollection AddInfrastructureSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApiDocument(options =>
        {
            options.Title = "txtOnly API";
            options.Version = "v1";
            options.SchemaSettings.GenerateXmlObjects = true;

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            options.Description = $"API RESTful do txtOnly. Assembly: {assembly.GetName().Name}";

            options.PostProcess = document =>
            {
                document.Info.Description = "API RESTful para publicação e interação com conteúdo social.";
            };

            options.AddSecurity(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Informe o token no formato: Bearer {seu_token}"
                });

            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        return services;
    }
}