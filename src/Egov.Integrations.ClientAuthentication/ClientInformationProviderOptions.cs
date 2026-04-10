using System.Security.Cryptography.X509Certificates;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Client information provider options.
/// </summary>
public class ClientInformationProviderOptions
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
    /// Time-to-live for cached client information.
    /// </summary>
    public TimeSpan CacheTtl { get; set; } = ClientAuthenticationDefaults.CacheTtl;

    /// <summary>
    /// Specifies whether to reuse any cached client information on API retrieval failure.
    /// </summary>
    public bool CacheReuseOnApiFailure { get; set; } = true;
}
