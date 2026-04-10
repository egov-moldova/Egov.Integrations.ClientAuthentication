using Egov.Integrations.ClientAuthentication;

namespace Microsoft.AspNetCore.Authorization;

/// <summary>
/// Extension methods for <see cref="AuthorizationPolicyBuilder"/>.
/// </summary>
public static class AuthorizationPolicyBuilderExtensions
{
    /// <summary>
    /// Adds a requirement that asserts against client settings for authorization.
    /// </summary>
    /// <typeparam name="TSettings">The type of client settings.</typeparam>
    /// <param name="builder">The <see cref="AuthorizationPolicyBuilder"/> to add the requirement to.</param>
    /// <param name="assertion">Settings assertion to be evaluated.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthorizationPolicyBuilder RequireSettingsAssertion<TSettings>(this AuthorizationPolicyBuilder builder, Func<TSettings, bool> assertion)
    {
        builder.Requirements.Add(new ClientAssertionRequirement<TSettings>(assertion));
        return builder;
    }

    /// <summary>
    /// Adds a requirement that asserts against client settings for authorization.
    /// </summary>
    /// <typeparam name="TSettings">The type of client settings.</typeparam>
    /// <param name="builder">The <see cref="AuthorizationPolicyBuilder"/> to add the requirement to.</param>
    /// <param name="assertion">Settings assertion to be evaluated.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static AuthorizationPolicyBuilder RequireSettingsAssertion<TSettings>(this AuthorizationPolicyBuilder builder, Func<TSettings, Task<bool>> assertion)
    {
        builder.Requirements.Add(new ClientAssertionRequirement<TSettings>(assertion));
        return builder;
    }
}
