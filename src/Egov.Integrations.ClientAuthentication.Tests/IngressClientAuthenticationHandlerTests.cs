using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Egov.Integrations.ClientAuthentication.Tests;

public class IngressClientAuthenticationHandlerTests
{
    private readonly Mock<IClientInformationProvider> _clientProviderMock = new();
    private readonly Mock<IOptionsMonitor<ClientAuthenticationOptions>> _optionsMonitorMock = new();
    private readonly Mock<ILoggerFactory> _loggerFactoryMock = new();
    private readonly Mock<ILogger> _loggerMock = new();
    private readonly UrlEncoder _encoder = UrlEncoder.Default;
    private readonly ClientAuthenticationOptions _options = new();
    private readonly string _schemeName = ClientAuthenticationDefaults.AuthenticationScheme;

    public IngressClientAuthenticationHandlerTests()
    {
        _optionsMonitorMock.Setup(m => m.Get(It.IsAny<string>())).Returns(_options);
        _loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsSuccess_WhenClientIdHeaderPresentAndValid()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new AuthenticatedClient { ID = clientId, Name = "Test Client" };
        var context = new DefaultHttpContext();
        context.Request.Headers[_options.IngressClientHeaderName] = clientId.ToString();

        _clientProviderMock.Setup(p => p.GetClientInformationAsync(_schemeName, clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var handler = new IngressClientAuthenticationHandler(_optionsMonitorMock.Object, _loggerFactoryMock.Object, _encoder, _clientProviderMock.Object);
        await handler.InitializeAsync(new AuthenticationScheme(_schemeName, _schemeName, typeof(IngressClientAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(clientId.ToString(), result.Principal?.FindFirstValue(ClaimTypes.Sid));
        Assert.Equal("Test Client", result.Principal?.Identity?.Name);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsNoResult_WhenNoHeadersPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var handler = new IngressClientAuthenticationHandler(_optionsMonitorMock.Object, _loggerFactoryMock.Object, _encoder, _clientProviderMock.Object);
        await handler.InitializeAsync(new AuthenticationScheme(_schemeName, _schemeName, typeof(IngressClientAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.True(result.None);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsSuccess_WhenCertSerialHeaderPresentAndValid()
    {
        // Arrange
        var serial = "123456789";
        var clientId = Guid.NewGuid();
        var client = new AuthenticatedClient { ID = clientId, Name = "Test Client" };
        var context = new DefaultHttpContext();
        context.Request.Headers[_options.IngressClientCertificateSerialHeaderName] = serial;

        _clientProviderMock.Setup(p => p.GetClientInformationByCertificateSerialAsync(_schemeName, serial, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var handler = new IngressClientAuthenticationHandler(_optionsMonitorMock.Object, _loggerFactoryMock.Object, _encoder, _clientProviderMock.Object);
        await handler.InitializeAsync(new AuthenticationScheme(_schemeName, _schemeName, typeof(IngressClientAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(clientId.ToString(), result.Principal?.FindFirstValue(ClaimTypes.Sid));
    }
}
