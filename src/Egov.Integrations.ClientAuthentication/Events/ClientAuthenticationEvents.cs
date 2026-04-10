namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Base events class that allows the application to override needed methods.
/// </summary>
public class ClientAuthenticationEvents
{
    /// <summary>
    /// A delegate assigned to this property will be invoked when client authentication is missing from request.
    /// </summary>
    public Func<ClientAuthenticationMissingContext, Task> OnClientMissing { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// A delegate assigned to this property will be invoked after client was authenticated.
    /// </summary>
    public Func<ClientAuthenticatedContext, Task> OnClientAuthenticated { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    public Func<ClientAuthenticationChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked when client authentication is missing.
    /// </summary>
    /// <param name="context">Used to report client authentication context and result.</param>
    public virtual Task ClientMissing(ClientAuthenticationMissingContext context) => OnClientMissing(context);

    /// <summary>
    /// Invoked after client was successfully authenticated.
    /// </summary>
    /// <param name="context">Used to report client authentication context and result.</param>
    public virtual Task ClientAuthenticated(ClientAuthenticatedContext context) => OnClientAuthenticated(context);

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    public virtual Task Challenge(ClientAuthenticationChallengeContext context) => OnChallenge(context);
}