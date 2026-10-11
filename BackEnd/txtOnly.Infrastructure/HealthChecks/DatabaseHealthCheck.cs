

using Microsoft.Extensions.Diagnostics.HealthChecks;
using txtOnly.Infrastructure.Context;

namespace txtOnly.Infrastructure.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public DatabaseHealthCheck(AppDbContext context)
    {
        _context = context;
    }



    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Banco de dados acessível.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Erro ao verificar o banco de dados.", ex);
        }
    }
}
