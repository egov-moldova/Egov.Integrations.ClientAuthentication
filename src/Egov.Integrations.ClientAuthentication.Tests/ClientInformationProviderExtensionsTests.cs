using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Egov.Integrations.ClientAuthentication.Tests;

public class ClientInformationProviderExtensionsTests
{
    private static X509Certificate2 CreateDummyCertificate()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("cn=test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
    }

    [Fact]
    public void AddClientInformationProvider_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var apiBaseAddress = new Uri("https://api.test");
        var scheme = "TestScheme";

        // Act
        services.AddClientInformationProvider(scheme, options =>
        {
            options.ApiBaseAddress = apiBaseAddress;
        });

        // We need this because of the .Configure<IOptions<Egov.Extensions.Configuration.SystemCertificateOptions>> call in the extension method
        services.Configure<Egov.Extensions.Configuration.SystemCertificateOptions>(_ => { });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var provider = serviceProvider.GetService<IClientInformationProvider>();
        Assert.NotNull(provider);
        Assert.IsType<MPassClientInformationProvider>(provider);

        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<ClientInformationProviderOptions>>();
        // NOTE: The extension method calls services.Configure(configureOptions) which configures the UNNAMED options
        var options = optionsMonitor.CurrentValue;
        Assert.Equal(apiBaseAddress, options.ApiBaseAddress);
    }
}
