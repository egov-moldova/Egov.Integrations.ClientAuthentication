using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Egov.Integrations.ClientAuthentication.Tests;

public class HttpContextExtensionsTests
{
    [Fact]
    public void GetAuthenticatedClient_ReturnsClient_WhenFeatureIsPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var client = new AuthenticatedClient { ID = Guid.NewGuid(), Name = "Test" };
        var scheme = ClientAuthenticationDefaults.AuthenticationScheme;
        var feature = new ClientAuthenticationFeature(scheme, client);
        context.Features.Set(feature);

        // Act
        var result = context.GetAuthenticatedClient();

        // Assert
        Assert.Same(client, result);
    }

    [Fact]
    public void GetAuthenticatedClient_ReturnsNull_WhenFeatureIsMissing()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var result = context.GetAuthenticatedClient();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAuthenticatedClient_WithSpecificScheme_ReturnsClient_WhenFeatureMatches()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var client = new AuthenticatedClient { ID = Guid.NewGuid(), Name = "Test" };
        var scheme = "CustomScheme";
        var feature = new ClientAuthenticationFeature(scheme, client);
        context.Features.Set(feature);

        // Act
        var result = context.GetAuthenticatedClient(scheme);

        // Assert
        Assert.Same(client, result);
    }

    [Fact]
    public void GetAuthenticatedClient_WithSpecificScheme_ReturnsNull_WhenFeatureDoesNotMatch()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var client = new AuthenticatedClient { ID = Guid.NewGuid(), Name = "Test" };
        var feature = new ClientAuthenticationFeature("OtherScheme", client);
        context.Features.Set(feature);

        // Act
        var result = context.GetAuthenticatedClient("CustomScheme");

        // Assert
        Assert.Null(result);
    }
}
