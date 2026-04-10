using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Client authentication options.
/// </summary>
public class ClientAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// The certificate to use as HTTP client certificate to authenticate with client information API.
    /// </summary>
    public X509Certificate2? SystemCertificate { get; set; }

    /// <summary>
    /// The intermediate certificates to use as HTTP client certificate chain to authenticate with client information API.
    /// </summary>
    public X509Certificate2Collection? SystemCertificateIntermediaries { get; set; }

    /// <summary>
    /// Base address of client information API.
    /// </summary>
    public Uri? ApiBaseAddress { get; set; }

    /// <summary>
    /// Expected ingress header for client identifier.
    /// </summary>
    public string IngressClientHeaderName { get; set; } = ClientAuthenticationDefaults.IngressClientHeaderName;

    /// <summary>
    /// Expected ingress header for client certificate serial number.
    /// </summary>
    public string IngressClientCertificateSerialHeaderName { get; set; } = ClientAuthenticationDefaults.IngressClientCertificateSerialHeaderName;

    /// <summary>
    /// Time-to-live for cached client information.
    /// </summary>
    public TimeSpan CacheTtl { get; set; } = ClientAuthenticationDefaults.CacheTtl;

    /// <summary>
    /// Specifies whether to reuse any cached client information on API retrieval failure.
    /// </summary>
    public bool CacheReuseOnApiFailure { get; set; } = true;

    /// <summary>
    /// Instance used for events.
    /// </summary>
    public new ClientAuthenticationEvents? Events
    {
        get => (ClientAuthenticationEvents?)base.Events;
        set => base.Events = value;
    }

    /// <inheritdoc/>
    public override void Validate()
    {
        if (SystemCertificate == null)
        {
            throw new InvalidOperationException("No system certificate configured");
        }
        if (!SystemCertificate.HasPrivateKey)
        {
            throw new InvalidOperationException("System certificate does not contain a private key");
        }

        if (ApiBaseAddress == null)
        {
            throw new InvalidOperationException($"No {nameof(ApiBaseAddress)} configured");
        }
    }
}
