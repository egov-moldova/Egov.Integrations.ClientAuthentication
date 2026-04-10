using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// State for challenge event.
/// </summary>
public class ClientAuthenticationChallengeContext : PropertiesContext<ClientAuthenticationOptions>
{
    /// <summary>
    /// Creates a new instance of <see cref="ClientAuthenticationChallengeContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> this context applies to.</param>
    /// <param name="scheme">The scheme used when the client authentication handler was registered.</param>
    /// <param name="options">Used <see cref="ClientAuthenticationOptions"/>.</param>
    /// <param name="properties">The authentication properties.</param>
    public ClientAuthenticationChallengeContext(
        HttpContext context,
        AuthenticationScheme scheme,
        ClientAuthenticationOptions options,
        AuthenticationProperties? properties)
        : base(context, scheme, options, properties)
    {
    }

    /// <summary>
    /// If true, will skip any default logic for this challenge.
    /// </summary>
    public bool Handled { get; private set; }

    /// <summary>
    /// Skips any default logic for this challenge.
    /// </summary>
    public void HandleResponse() => Handled = true;
}
