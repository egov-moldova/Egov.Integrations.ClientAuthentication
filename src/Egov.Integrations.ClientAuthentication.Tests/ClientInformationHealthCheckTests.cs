using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Egov.Integrations.ClientAuthentication.Tests;

public class ClientInformationHealthCheckTests
{
    private static X509Certificate2 CreateDummyCertificate(DateTimeOffset notAfter)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("cn=test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var notBefore = notAfter.AddDays(-1) < DateTimeOffset.Now ? notAfter.AddDays(-1) : DateTimeOffset.Now.AddDays(-1);
        return request.CreateSelfSigned(notBefore, notAfter);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenCertificateIsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ClientInformationHealthCheck>>();
        var optionsMonitorMock = new Mock<IOptionsMonitor<ClientInformationProviderOptions>>();
        var options = new ClientInformationProviderOptions { SystemCertificate = null };
        var scheme = "TestScheme";

        optionsMonitorMock.Setup(m => m.Get(scheme)).Returns(options);

        var healthCheck = new ClientInformationHealthCheck(loggerMock.Object, optionsMonitorMock.Object);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(scheme, healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("No system certificate specified", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsUnhealthy_WhenCertificateIsExpired()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ClientInformationHealthCheck>>();
        var optionsMonitorMock = new Mock<IOptionsMonitor<ClientInformationProviderOptions>>();
        using var cert = CreateDummyCertificate(DateTimeOffset.Now.AddDays(-1));
        var options = new ClientInformationProviderOptions { SystemCertificate = cert };
        var scheme = "TestScheme";

        optionsMonitorMock.Setup(m => m.Get(scheme)).Returns(options);

        var healthCheck = new ClientInformationHealthCheck(loggerMock.Object, optionsMonitorMock.Object);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(scheme, healthCheck, HealthStatus.Unhealthy, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("System certificate is expired", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenCertificateIsValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ClientInformationHealthCheck>>();
        var optionsMonitorMock = new Mock<IOptionsMonitor<ClientInformationProviderOptions>>();
        using var cert = CreateDummyCertificate(DateTimeOffset.Now.AddDays(40));
        var options = new ClientInformationProviderOptions { SystemCertificate = cert };
        var scheme = "TestScheme";

        optionsMonitorMock.Setup(m => m.Get(scheme)).Returns(options);

        var healthCheck = new ClientInformationHealthCheck(loggerMock.Object, optionsMonitorMock.Object);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(scheme, healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }
}
