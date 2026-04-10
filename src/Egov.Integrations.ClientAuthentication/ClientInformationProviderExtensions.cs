using System.Net.Security;
using Egov.Extensions.Configuration;
using Egov.Integrations.ClientAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods to add client information provider to an application.
/// </summary>
public static class ClientInformationProviderExtensions
{
    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(this IServiceCollection services)
        => services.AddClientInformationProvider(ClientAuthenticationDefaults.AuthenticationScheme);

    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(this IServiceCollection services, string authenticationScheme)
        => services.AddClientInformationProvider(authenticationScheme, _ => { });

    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(this IServiceCollection services, IConfiguration config)
        => services.AddClientInformationProvider(ClientAuthenticationDefaults.AuthenticationScheme, config);

    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientInformationProviderOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(
        this IServiceCollection services,
        IConfiguration config,
        Action<ClientInformationProviderOptions> configureOptions)
        => services.AddClientInformationProvider(ClientAuthenticationDefaults.AuthenticationScheme, config, configureOptions);

    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(this IServiceCollection services, string authenticationScheme, IConfiguration config)
        => services.AddClientInformationProvider(authenticationScheme, config, _ => { });

    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="config">The configuration being bound to.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientInformationProviderOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(
        this IServiceCollection services,
        string authenticationScheme,
        IConfiguration config,
        Action<ClientInformationProviderOptions> configureOptions)
    {
        services.Configure<ClientInformationProviderOptions>(authenticationScheme, config);
        return services.AddClientInformationProvider(authenticationScheme, configureOptions);
    }

    /// <summary>
    /// Adds an implementation for <see cref="IClientInformationProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ClientInformationProviderOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddClientInformationProvider(
        this IServiceCollection services,
        string authenticationScheme,
        Action<ClientInformationProviderOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddOptions<ClientInformationProviderOptions>(authenticationScheme)
            .Configure<IOptions<SystemCertificateOptions>>((clientInformationProviderOptions, systemCertificateOptions) =>
            {
                var systemCertificateOptionsValue = systemCertificateOptions.Value;
                clientInformationProviderOptions.SystemCertificate ??= systemCertificateOptionsValue.Certificate;
                clientInformationProviderOptions.SystemCertificateIntermediaries ??= systemCertificateOptionsValue.IntermediateCertificates;
            });

        services.AddHttpClient();
        services.AddOptions<HttpClientFactoryOptions>(MPassClientInformationProvider.HttpClientNamePrefix + authenticationScheme)
            .Configure<IOptionsMonitor<ClientInformationProviderOptions>>((clientFactoryOptions, clientInformationProviderOptions) =>
            {
                clientFactoryOptions.HttpClientActions.Add(client =>
                {
                    client.BaseAddress = clientInformationProviderOptions.Get(authenticationScheme).ApiBaseAddress;
                });

                clientFactoryOptions.HttpMessageHandlerBuilderActions.Add(client =>
                {
                    var clientOptions = clientInformationProviderOptions.Get(authenticationScheme);
                    client.PrimaryHandler = new SocketsHttpHandler
                    {
                        SslOptions = new SslClientAuthenticationOptions
                        {
                            ClientCertificateContext = SslStreamCertificateContext.Create(clientOptions.SystemCertificate!, clientOptions.SystemCertificateIntermediaries, true)
                        }
                    };
                });
            });

        services.TryAddSingleton<IClientInformationProvider, MPassClientInformationProvider>();

        return services;
    }
}