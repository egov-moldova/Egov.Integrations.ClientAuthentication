using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Egov.Integrations.ClientAuthentication;

internal class IngressClientAuthenticationHandler : AuthenticationHandler<ClientAuthenticationOptions>
{
    private readonly IClientInformationProvider _clientProvider;

    public IngressClientAuthenticationHandler(
        IOptionsMonitor<ClientAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IClientInformationProvider clientProvider) : base(options, logger, encoder)
    {
        _clientProvider = clientProvider;
    }

    /// <inheritdoc/>
    protected new ClientAuthenticationEvents Events
    {
        get => (ClientAuthenticationEvents)base.Events!;
        set => base.Events = value;
    }

    /// <inheritdoc/>
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new ClientAuthenticationEvents());

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticatedClient? client;

        // try client header
        var headerValues = Context.Request.Headers[Options.IngressClientHeaderName];
        if ((headerValues.Count != 1) || !Guid.TryParse(headerValues[0], out var clientId))
        {
            Logger.NoIngressHeader(Options.IngressClientHeaderName);

            // then try certificate serial header
            headerValues = Context.Request.Headers[Options.IngressClientCertificateSerialHeaderName];
            if ((headerValues.Count != 1) || string.IsNullOrWhiteSpace(headerValues[0]))
            {
                Logger.NoIngressHeader(Options.IngressClientCertificateSerialHeaderName);

                var authenticationMissingContext = new ClientAuthenticationMissingContext(Context, Scheme, Options);
                await Events.ClientMissing(authenticationMissingContext);
                if (authenticationMissingContext.Result != null)
                {
                    if (authenticationMissingContext.Result.Succeeded && (authenticationMissingContext.Client != null))
                    {
                        Context.Features.Set(new ClientAuthenticationFeature(Scheme.Name, authenticationMissingContext.Client));
                    }

                    return authenticationMissingContext.Result;
                }

                return AuthenticateResult.NoResult();
            }

            var clientCertificateSerial = headerValues.ToString();
            var cancellationToken = Context.WebSockets.IsWebSocketRequest ? CancellationToken.None : Context.RequestAborted;
            client = await _clientProvider.GetClientInformationByCertificateSerialAsync(Scheme.Name, clientCertificateSerial, cancellationToken);

            if (client == null)
            {
                Logger.ClientInformationNotFoundByCertificateSerial(clientCertificateSerial);
                return AuthenticateResult.NoResult();
            }
        }
        else
        {
            try
            {
                var cancellationToken = Context.WebSockets.IsWebSocketRequest ? CancellationToken.None : Context.RequestAborted;
                client = await _clientProvider.GetClientInformationAsync(Scheme.Name, clientId, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.FailedRetrievingClientInformation(ex);
                return AuthenticateResult.Fail(ex);
            }

            if (client == null)
            {
                Logger.ClientInformationNotFound(clientId);
                return AuthenticateResult.NoResult();
            }
        }

        var authenticatedContext = new ClientAuthenticatedContext(Context, Scheme, Options)
        {
            Client = client,
            Principal = client.CreatePrincipal(Scheme.Name)
        };
        await Events.ClientAuthenticated(authenticatedContext);

        if (authenticatedContext.Result == null)
        {
            authenticatedContext.Success();
        }

        if (authenticatedContext.Result!.Succeeded)
        {
            Context.Features.Set(new ClientAuthenticationFeature(Scheme.Name, authenticatedContext.Client ?? client));
        }
        return authenticatedContext.Result;
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authenticationChallengedContext = new ClientAuthenticationChallengeContext(Context, Scheme, Options, properties);
        await Events.Challenge(authenticationChallengedContext);

        if (authenticationChallengedContext.Handled)
        {
            return;
        }

        // Ingress-based authentication is used for system authentications.
        // We can't prompt user for authentication, so the best thing to do is Forbid, not Challenge.
        await HandleForbiddenAsync(properties);
    }
}
