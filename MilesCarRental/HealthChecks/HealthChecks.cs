namespace MilesCarRental.Api.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
 HealthCheckContext context,
     CancellationToken cancellationToken = default)
    {
        // Para InMemory siempre está "healthy"
        // En una implementación real, verificarías la conexión a la BD
        try
        {
            // Simular check de base de datos
            var isHealthy = true;

            if (isHealthy)
            {
                return Task.FromResult(
            HealthCheckResult.Healthy("In-memory database is available"));
            }

            return Task.FromResult(
              new HealthCheckResult(
           context.Registration.FailureStatus,
             "Database is unavailable"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
            new HealthCheckResult(
              context.Registration.FailureStatus,
                exception: ex));
        }
    }
}

public class ApiHealthCheck : IHealthCheck
{
    private readonly ILogger<ApiHealthCheck> _logger;

    public ApiHealthCheck(ILogger<ApiHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
      CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar estado general de la API
            var healthData = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["uptime"] = TimeSpan.FromMilliseconds(Environment.TickCount64)
            };

            return Task.FromResult(
                 HealthCheckResult.Healthy(
                    "API is running",
                 data: healthData));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return Task.FromResult(
                new HealthCheckResult(
               context.Registration.FailureStatus,
                  exception: ex));
        }
    }
}
