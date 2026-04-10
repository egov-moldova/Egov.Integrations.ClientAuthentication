namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Default values related to client authentication middleware.
/// </summary>
public static class ClientAuthenticationDefaults
{
    /// <summary>
    /// The default value used for client authentication scheme.
    /// </summary>
    public const string AuthenticationScheme = "ClientAuthentication";

    /// <summary>
    /// Default name of the header provided by ingress for client identifier.
    /// </summary>
    public const string IngressClientHeaderName = "X-MPass-Client-ID";

    /// <summary>
    /// Default name of the header provided by ingress for client certificate serial number.
    /// </summary>
    public const string IngressClientCertificateSerialHeaderName = "X-MPass-Client-Cert-Serial";

    /// <summary>
    /// Default cache time-to-live setting.
    /// </summary>
    public static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);
}
