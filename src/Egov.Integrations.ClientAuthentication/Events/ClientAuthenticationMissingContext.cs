using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// State for missing authentication context.
/// </summary>
public class ClientAuthenticationMissingContext : ResultContext<ClientAuthenticationOptions>
{
    /// <summary>
    /// Creates a new instance of <see cref="ClientAuthenticationMissingContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> this context applies to.</param>
    /// <param name="scheme">The scheme used when the client authentication handler was registered.</param>
    /// <param name="options">Used <see cref="ClientAuthenticationOptions"/>.</param>
    public ClientAuthenticationMissingContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ClientAuthenticationOptions options)
        : base(context, scheme, options)
    {
    }

    /// <summary>
    /// The authenticated client.
    /// </summary>
    public AuthenticatedClient? Client { get; private set; }

    /// <summary>
    /// Calls success creating a ticket with the provided <paramref name="client"/>.
    /// </summary>
    /// <param name="client">Client to be considered as authenticated.</param>
    public void Success(AuthenticatedClient client)
    {
        Client = client;
        Principal ??= client.CreatePrincipal(Scheme.Name);
        Success();
    }
}
