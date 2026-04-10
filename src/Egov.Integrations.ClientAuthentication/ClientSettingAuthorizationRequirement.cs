using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Egov.Integrations.ClientAuthentication;

internal sealed class ClientAssertionRequirement<TSettings> : AuthorizationHandler<ClientAssertionRequirement<TSettings>, HttpContext>, IAuthorizationRequirement
{
    private readonly Func<TSettings, Task<bool>> _settingsAssertion;

    public ClientAssertionRequirement(Func<TSettings, bool> settingsAssertion)
    {
        if (settingsAssertion == null)
        {
            throw new ArgumentNullException(nameof(settingsAssertion));
        }

        _settingsAssertion = settings => Task.FromResult(settingsAssertion(settings));
    }

    public ClientAssertionRequirement(Func<TSettings, Task<bool>> settingsAssertion)
    {
        _settingsAssertion = settingsAssertion ?? throw new ArgumentNullException(nameof(settingsAssertion));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientAssertionRequirement<TSettings> requirement, HttpContext resource)
    {
        // TODO: Optimize potentially repeated settings deserialization

        if ((resource.GetAuthenticatedClient() is { } authenticatedClient) &&
            (authenticatedClient.DeserializeSettings<TSettings>() is { } settings) &&
            await _settingsAssertion(settings))
        {
            context.Succeed(this);
        }
    }
}
