

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace txtOnly.CrossCutting.IoC;

public static class DependencyInjectionRateLimiter
{
    /// <summary>
    /// Configura o Rate Limiter da aplicação.
    /// O Rate Limiter limita a quantidade de requisições
    /// que um cliente pode realizar em determinado período.
    /// </summary>
    public static IServiceCollection AddInfrastructureRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            // Define o código HTTP retornado quando o limite de requisições é atingido.
            // 429 = Too Many Requests (Muitas requisições).
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;


            // ============================================================
            // LIMITE GERAL DA API
            // ============================================================

            options.AddFixedWindowLimiter("fixedwindow", limiterOptions =>
            {
                // Permite no máximo 80 requisições durante a janela definida abaixo.
                limiterOptions.PermitLimit = 80;

                // Define uma janela de tempo de 10 segundos.
                //
                // Exemplo:
                // O cliente pode fazer até 80 requisições em 10 segundos.
                //
                // Após os 10 segundos, a contagem é reiniciada e
                // ele poderá realizar novamente até 80 requisições.
                limiterOptions.Window = TimeSpan.FromSeconds(10);

                // Define a ordem da fila caso requisições sejam colocadas em espera.
                // OldestFirst = as requisições mais antigas seriam processadas primeiro.
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

                // Quantidade de requisições que podem ficar esperando na fila.
                //
                // 0 significa que nenhuma requisição ficará aguardando.
                // Quando o limite for atingido, a requisição será rejeitada imediatamente.
                limiterOptions.QueueLimit = 0;
            });


            // ============================================================
            // LIMITE ESPECÍFICO PARA LOGIN
            // ============================================================

            options.AddFixedWindowLimiter("loginRateLimit", limiterOptions =>
            {
                // Permite no máximo 3 tentativas de login.
                limiterOptions.PermitLimit = 3;

                // O período para essas 3 tentativas é de 1 minuto.
                //
                // Exemplo:
                // O usuário pode tentar fazer login 3 vezes durante 1 minuto.
                //
                // Ao atingir a quarta tentativa dentro desse período,
                // receberá o erro 429.
                //
                // Após a janela de 1 minuto terminar,
                // a contagem é reiniciada.
                limiterOptions.Window = TimeSpan.FromMinutes(1);

                // Processaria primeiro as requisições mais antigas
                // caso existisse uma fila.
                limiterOptions.QueueProcessingOrder =
                    QueueProcessingOrder.OldestFirst;

                // Não permite requisições esperando em uma fila.
                //
                // Ao atingir o limite, a requisição será rejeitada imediatamente.
                limiterOptions.QueueLimit = 0;
            });


            // ============================================================
            // RESPOSTA PERSONALIZADA QUANDO O LIMITE É ATINGIDO
            // ============================================================

            options.OnRejected = async (context, cancellationToken) =>
            {
                // Define o status HTTP 429.
                //
                // 429 = Too Many Requests
                // Muitas requisições/tentativas.
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;


                // Informa ao cliente quanto tempo deve esperar antes
                // de tentar novamente.
                //
                // 60 segundos = 1 minuto.
                context.HttpContext.Response.Headers["Retry-After"] = "60";


                // Retorna uma resposta JSON personalizada para o frontend.
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        status = 429,

                        // Mensagem exibida para o usuário.
                        message = "Muitas tentativas. Tente novamente em 1 minuto."
                    },
                    cancellationToken);
            };
        });

        // Retorna os serviços configurados para permitir
        // o encadeamento da configuração.
        return services;
    }
}
