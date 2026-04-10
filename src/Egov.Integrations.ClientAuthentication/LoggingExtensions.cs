using Microsoft.Extensions.Logging;

namespace Egov.Integrations.ClientAuthentication;

internal static partial class LoggingExtensions
{
    [LoggerMessage(0, LogLevel.Debug, "HTTP header '{HeaderName}' not found or has invalid value.")]
    public static partial void NoIngressHeader(this ILogger logger, string headerName);

    [LoggerMessage(1, LogLevel.Warning, "Error on retrieving client information")]
    public static partial void FailedRetrievingClientInformation(this ILogger logger, Exception exception);

    [LoggerMessage(2, LogLevel.Warning, "Information for client '{ClientId}' not found.")]
    public static partial void ClientInformationNotFound(this ILogger logger, Guid clientId);

    [LoggerMessage(3, LogLevel.Warning, "Information for client '{ClientCertificateSerial}' not found.")]
    public static partial void ClientInformationNotFoundByCertificateSerial(this ILogger logger, string clientCertificateSerial);
}
