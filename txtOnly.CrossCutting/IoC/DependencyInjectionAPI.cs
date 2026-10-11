using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using txtOnly.Application.Interfaces;
using txtOnly.Application.Services;
using txtOnly.Domain.Interfaces;
using txtOnly.Infrastructure.Context;
using txtOnly.Infrastructure.HealthChecks;
using txtOnly.Infrastructure.Repositories;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionAPI
{
    public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
    {
        // ===============================
        // CONTEXTO DO BANCO DE DADOS. Usando sql Server
        // ===============================
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );




        // ===============================
        // CONFIGURAÇÃO DE IDENTIDADE
        // ===============================
        services.AddInfrastructureIdentity();




        // ===============================
        // AUTORIZAÇÃO E TOKEN
        // ===============================
        services.AddJwtAuthentication(configuration);




        // ===============================
        // VERIFICAÇÃO DE SAÚDE
        // ===============================
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");




        // ===============================
        // SERVIÇOS DA APLICAÇÃO
        // ===============================
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService>(serviceProvider =>
        {
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new UserService(uow, uow.UserRepository);
        });
        services.AddScoped<IPostService>(serviceProvider =>
        {
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new PostService(uow, uow.PostRepository, uow.UserRepository);
        });
        services.AddScoped<ICommentService>(serviceProvider =>
        {
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new CommentService(uow, uow.CommentRepository,
               uow.UserRepository, uow.PostRepository);
        });
        services.AddScoped<ILikeService>(serviceProvider =>
        {
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new LikeService(uow, uow.LikeRepository,
               uow.UserRepository, uow.PostRepository);
        });
        services.AddScoped<IFollowService>(serviceProvider =>
        {
            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();
            return new FollowService(uow, uow.FollowRepository, uow.UserRepository);
        });

        return services;
    }

}
