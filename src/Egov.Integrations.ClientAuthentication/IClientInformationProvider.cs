namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Interface that defines provides for authenticated client information.
/// </summary>
public interface IClientInformationProvider
{
    /// <summary>
    /// Retrieves client information by its identifier.
    /// </summary>
    /// <param name="clientId">Client identifier.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> to observe.</param>
    /// <returns>An instance of <see cref="AuthenticatedClient"/> or null, if client not available.</returns>
    /// <exception cref="HttpRequestException">Thrown when client information cannot be retrieved and cache reuse on failure is disabled.</exception>
    Task<AuthenticatedClient?> GetClientInformationAsync(Guid clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves client information by authentication scheme and client identifier.
    /// </summary>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="clientId">Client identifier.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> to observe.</param>
    /// <returns>An instance of <see cref="AuthenticatedClient"/> or null, if client not found.</returns>
    /// <exception cref="HttpRequestException">Thrown when client information cannot be retrieved and cache reuse on failure is disabled.</exception>
    Task<AuthenticatedClient?> GetClientInformationAsync(string authenticationScheme, Guid clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves client information by its certificate serial number.
    /// </summary>
    /// <param name="clientCertificateSerial">Client certificate serial number.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> to observe.</param>
    /// <returns>An instance of <see cref="AuthenticatedClient"/> or null, if client not found.</returns>
    /// <exception cref="HttpRequestException">Thrown when client information cannot be retrieved and cache reuse on failure is disabled.</exception>
    Task<AuthenticatedClient?> GetClientInformationByCertificateSerialAsync(string clientCertificateSerial, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves client information by authentication scheme and client certificate serial number.
    /// </summary>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="clientCertificateSerial">Client certificate serial number.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> to observe.</param>
    /// <returns>An instance of <see cref="AuthenticatedClient"/> or null, if client not available.</returns>
    /// <exception cref="HttpRequestException">Thrown when client information cannot be retrieved and cache reuse on failure is disabled.</exception>
    Task<AuthenticatedClient?> GetClientInformationByCertificateSerialAsync(string authenticationScheme, string clientCertificateSerial, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves information for all clients.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> to observe.</param>
    /// <returns>List of all <see cref="AuthenticatedClient"/>.</returns>
    /// <exception cref="HttpRequestException">Thrown when client information cannot be retrieved and cache reuse on failure is disabled.</exception>
    Task<IList<AuthenticatedClient>> GetAllClientsInformationAsync(CancellationToken cancellationToken = default);
}