using Egov.Extensions.Configuration;
using Egov.Integrations.ClientAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods to add client authentication capabilities to an HTTP application pipeline.
/// </summary>
public static class ClientAuthenticationExtensions
{
    /// <summary>
    /// Adds core authentication services and ingress-based client authentication.
    /// Use when other services don't require data protection.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClientAuthenticationCore(this IServiceCollection services, IConfiguration config)
        => services.AddIngressClientAuthenticationCore(config, configureOptions: null);

    /// <summary>
    /// Adds core authentication services and ingress-based client authentication.
    /// Use when other services don't require data protection.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClientAuthenticationCore(
        this IServiceCollection services,
        Action<ClientAuthenticationOptions>? configureOptions)
    {
        // inspired from: https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/Core/src/AuthenticationServiceCollectionExtensions.cs
        services.AddAuthenticationCore(options => options.DefaultScheme = ClientAuthenticationDefaults.AuthenticationScheme);
        services.AddWebEncoders();
        services.TryAddSingleton(TimeProvider.System);

        return new AuthenticationBuilder(services)
            .AddIngressClient(configureOptions);
    }

    /// <summary>
    /// Adds core authentication services and ingress-based client authentication.
    /// Use when other services don't require data protection.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClientAuthenticationCore(
        this IServiceCollection services,
        IConfiguration config,
        Action<ClientAuthenticationOptions>? configureOptions)
    {
        services.Configure<ClientAuthenticationOptions>(ClientAuthenticationDefaults.AuthenticationScheme, config);
        return services.AddIngressClientAuthenticationCore(configureOptions);
    }

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(this AuthenticationBuilder builder)
        => builder.AddIngressClient(ClientAuthenticationDefaults.AuthenticationScheme);

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(this AuthenticationBuilder builder, string authenticationScheme)
        => builder.AddIngressClient(authenticationScheme, configureOptions: null);

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(this AuthenticationBuilder builder, IConfiguration config)
        => builder.AddIngressClient(ClientAuthenticationDefaults.AuthenticationScheme, config);

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(
        this AuthenticationBuilder builder,
        IConfiguration config,
        Action<ClientAuthenticationOptions>? configureOptions)
        => builder.AddIngressClient(ClientAuthenticationDefaults.AuthenticationScheme, config, configureOptions);

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(this AuthenticationBuilder builder, 
        string authenticationScheme,
        IConfiguration config)
        => builder.AddIngressClient(authenticationScheme, config, configureOptions: null);

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(
        this AuthenticationBuilder builder, 
        string authenticationScheme,
        IConfiguration config,
        Action<ClientAuthenticationOptions>? configureOptions)
    {
        builder.Services.Configure<ClientAuthenticationOptions>(authenticationScheme, config);
        return builder.AddIngressClient(authenticationScheme, configureOptions);
    }

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(
        this AuthenticationBuilder builder, 
        Action<ClientAuthenticationOptions>? configureOptions)
        => builder.AddIngressClient(ClientAuthenticationDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Adds ingress-based client authentication.
    /// Note that this also adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddIngressClient(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<ClientAuthenticationOptions>? configureOptions)
    {
        builder.Services.AddOptions<ClientAuthenticationOptions>(authenticationScheme)
            .Configure<IOptions<SystemCertificateOptions>>((clientAuthenticationOptions, systemCertificateOptions) =>
            {
                var systemCertificateOptionsValue = systemCertificateOptions.Value;
                clientAuthenticationOptions.SystemCertificate ??= systemCertificateOptionsValue.Certificate;
                clientAuthenticationOptions.SystemCertificateIntermediaries ??= systemCertificateOptionsValue.IntermediateCertificates;
            });

        builder.Services.AddOptions<ClientInformationProviderOptions>(authenticationScheme)
            .Configure<IOptionsMonitor<ClientAuthenticationOptions>>((clientInformationProviderOptions, clientAuthenticationOptions) =>
            {
                var clientAuthenticationOptionsValue = clientAuthenticationOptions.Get(authenticationScheme);
                clientInformationProviderOptions.SystemCertificate ??= clientAuthenticationOptionsValue.SystemCertificate;
                clientInformationProviderOptions.SystemCertificateIntermediaries = clientAuthenticationOptionsValue.SystemCertificateIntermediaries;
                clientInformationProviderOptions.ApiBaseAddress = clientAuthenticationOptionsValue.ApiBaseAddress;
                clientInformationProviderOptions.CacheTtl = clientAuthenticationOptionsValue.CacheTtl;
                clientInformationProviderOptions.CacheReuseOnApiFailure = clientAuthenticationOptionsValue.CacheReuseOnApiFailure;
            });

        builder.Services.AddClientInformationProvider(authenticationScheme);

        return builder.AddScheme<ClientAuthenticationOptions, IngressClientAuthenticationHandler>(authenticationScheme, configureOptions);
    }
}
