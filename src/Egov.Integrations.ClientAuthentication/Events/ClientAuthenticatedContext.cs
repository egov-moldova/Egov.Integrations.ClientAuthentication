using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// State for client authenticated event.
/// </summary>
public class ClientAuthenticatedContext : ResultContext<ClientAuthenticationOptions>
{
    /// <summary>
    /// Creates a new instance of <see cref="ClientAuthenticatedContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> this context applies to.</param>
    /// <param name="scheme">The scheme used when the client authentication handler was registered.</param>
    /// <param name="options">Used <see cref="ClientAuthenticationOptions"/>.</param>
    public ClientAuthenticatedContext(
        HttpContext context,
        AuthenticationScheme scheme, 
        ClientAuthenticationOptions options) 
        : base(context, scheme, options)
    {
    }

    /// <summary>
    /// The authenticated client.
    /// </summary>
    public AuthenticatedClient Client { get; set; } = default!;
}