using Egov.Integrations.ClientAuthentication;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IHealthChecksBuilder"/>.
/// </summary>
public static class HealthChecksBuilderExtensions
{
    /// <summary>
    /// Adds a new health check for client information provider. In particular, it checks for system certificate expiration.
    /// </summary>
    /// <param name="builder">An instance of <see cref="IHealthChecksBuilder"/> to add the health check to.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check reports a failure, or <see cref="HealthStatus.Unhealthy"/> if null.</param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/> so that additional calls can be chained.</returns>
    public static IHealthChecksBuilder AddClientInformationProviderHealthCheck(
        this IHealthChecksBuilder builder,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = null)
        => builder.AddClientInformationProviderHealthCheck(ClientAuthenticationDefaults.AuthenticationScheme, failureStatus, tags);

    /// <summary>
    /// Adds a new health check for client information API. In particular, it checks for system certificate expiration.
    /// </summary>
    /// <param name="builder">An instance of <see cref="IHealthChecksBuilder"/> to add the health check to.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check reports a failure, or <see cref="HealthStatus.Unhealthy"/> if null.</param>
    /// <param name="tags">A list of tags that can be used to filter health checks.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/> so that additional calls can be chained.</returns>
    public static IHealthChecksBuilder AddClientInformationProviderHealthCheck(this IHealthChecksBuilder builder, string authenticationScheme, HealthStatus? failureStatus = default, IEnumerable<string>? tags = null)
    {
        builder.Services.AddSingleton<ClientInformationHealthCheck>();
        return builder.AddCheck<ClientInformationHealthCheck>(authenticationScheme, failureStatus, tags, null);
    }
}
