

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using txtOnly.Domain.Validation;

namespace txtOnly.API.Middleware;


/// <summary>
/// Middleware responsável por capturar exceções não tratadas durante o pipeline da requisição HTTP
/// e retornar respostas padronizadas no formato RFC 7807 (ProblemDetails).
/// </summary>

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Construtor para injeção de dependências do middleware.
    /// </summary>
    /// <param name="next">Próximo delegado/middleware no pipeline HTTP.</param>
    /// <param name="logger">Serviço de logging para registrar erros ou avisos.</param>
    /// <param name="environment">Informa o ambiente de execução (ex: Desenvolvimento, Produção).</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }


    /// <summary>
    /// Método invocado automaticamente em cada requisição HTTP.
    /// Tenta executar o fluxo normal da aplicação e intercepta qualquer exceção lançada.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Passa a requisição para o próximo middleware no pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Captura qualquer exceção lançada e aciona o tratamento personalizado
            await HandleExceptionAsync(context, ex);
        }
    }


    /// <summary>
    /// Trata a exceção capturada, definindo o código de status HTTP, gravando logs
    /// e estruturando a resposta JSON de erro.
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Mapeia o tipo de exceção capturada para o código HTTP apropriado
        var statusCode = exception switch
        {
            DomainExceptionValidation => HttpStatusCode.BadRequest, // Regras de negócio/validação -> 400
            KeyNotFoundException => HttpStatusCode.NotFound,
            InvalidOperationException => HttpStatusCode.Conflict,
            DbUpdateException => HttpStatusCode.Conflict,           // Conflitos no banco de dados -> 409
            ArgumentNullException => HttpStatusCode.BadRequest,     // Parâmetro nulo -> 400
            ArgumentException => HttpStatusCode.BadRequest,         // Parâmetro inválido -> 400
            _ => HttpStatusCode.InternalServerError                 // Erro não previsto -> 500
        };

        // Grava o log de acordo com a gravidade do erro
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            // Erros não previstos (500) geram logs de nível Error com stack trace
            _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            // Erros esperados de negócio ou requisição geram apenas avisos (Warning)
            _logger.LogWarning(exception, "Handled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }


        // Monta o objeto ProblemDetails com os detalhes do erro para o cliente
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = GetTitle(statusCode),
            // Oculta detalhes do erro 500 se não estiver no ambiente de Desenvolvimento (por segurança)
            Detail = statusCode == HttpStatusCode.InternalServerError && !_environment.IsDevelopment()
                ? "Ocorreu um erro inesperado."
                : exception.Message,
            Instance = context.Request.Path
        };

        // Configura o status da resposta HTTP e o cabeçalho Content-Type correto para erros padronizados
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        // Serializa e envia o objeto ProblemDetails como resposta JSON
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    /// <summary>
    /// Retorna um título genérico amigável com base no código de status HTTP.
    /// </summary>
    private static string GetTitle(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Requisição inválida",
            HttpStatusCode.NotFound => "Recurso não encontrado",
            HttpStatusCode.Conflict => "Conflito ao processar a requisição",
            _ => "Erro interno do servidor"
        };
    }
}
