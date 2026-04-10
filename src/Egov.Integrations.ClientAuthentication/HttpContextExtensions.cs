using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Extensions for <see cref="HttpContext"/> to retrieve authenticated client information.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Retrieves authenticated client information, if any.
    /// Note that when using <see cref="AuthorizeAttribute"/>, the returned value is always non-null.
    /// </summary>
    /// <returns>An instance of <see cref="AuthenticatedClient"/> or null, if calling client is not authenticated.</returns>
    public static AuthenticatedClient? GetAuthenticatedClient(this HttpContext context)
    {
        return context.GetAuthenticatedClient(ClientAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Retrieves authenticated client information, if any.
    /// Note that when using <see cref="AuthorizeAttribute"/>, the returned value is always non-null.
    /// </summary>
    /// <returns>An instance of <see cref="AuthenticatedClient"/> or null, if calling client is not authenticated.</returns>
    public static AuthenticatedClient? GetAuthenticatedClient(this HttpContext context, string authenticationScheme)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context), "No HttpContext provided");
        }

        if ((context.Features.Get<ClientAuthenticationFeature>() is { } feature) &&
            (feature.AuthenticationScheme == authenticationScheme))
        {
            return feature.Client;
        }

        return null;
    }
}
