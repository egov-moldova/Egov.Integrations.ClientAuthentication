using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Egov.Integrations.ClientAuthentication;

internal class ClientInformationHealthCheck : IHealthCheck
{
    private readonly ILogger<ClientInformationHealthCheck> _logger;
    private readonly IOptionsMonitor<ClientInformationProviderOptions> _optionsMonitor;

    public ClientInformationHealthCheck(
        ILogger<ClientInformationHealthCheck> logger,
        IOptionsMonitor<ClientInformationProviderOptions> optionsMonitor)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var options = _optionsMonitor.Get(context.Registration.Name);

        if (options.SystemCertificate == null)
            return LogResult(HealthCheckResult.Unhealthy("No system certificate specified"));

        if (!options.SystemCertificate.HasPrivateKey)
            return LogResult(HealthCheckResult.Unhealthy("System certificate does not contain private key"));

        if (options.SystemCertificate.NotAfter < DateTime.Now)
            return LogResult(new HealthCheckResult(context.Registration.FailureStatus, "System certificate is expired"));

        if (options.SystemCertificate.NotAfter < DateTime.Now.AddDays(30))
            return LogResult(HealthCheckResult.Degraded("System certificate expires in less than 30 days"));

        return LogResult(HealthCheckResult.Healthy("Client information health check success"));
    }

    private Task<HealthCheckResult> LogResult(HealthCheckResult result)
    {
        var logLevel = result.Status switch
        {
            HealthStatus.Unhealthy => LogLevel.Critical,
            HealthStatus.Degraded => LogLevel.Warning,
            HealthStatus.Healthy => LogLevel.Trace,
            _ => throw new NotImplementedException()
        };

        _logger.Log(logLevel, result.Description);

        return Task.FromResult(result);
    }
}
