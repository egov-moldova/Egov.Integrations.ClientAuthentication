using System.Security.Claims;

namespace Egov.Integrations.ClientAuthentication.Tests;

public class AuthenticatedClientTests
{
    [Fact]
    public void DeserializeSettings_WithNullSettings_ReturnsDefault()
    {
        // Arrange
        var client = new AuthenticatedClient { Settings = null };

        // Act
        var settings = client.DeserializeSettings<TestSettings>();

        // Assert
        Assert.Null(settings);
    }

    [Fact]
    public void DeserializeSettings_WithValidJson_ReturnsDeserializedObject()
    {
        // Arrange
        var client = new AuthenticatedClient { Settings = "{\"Prop1\":\"Value1\",\"Prop2\":123}" };

        // Act
        var settings = client.DeserializeSettings<TestSettings>();

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("Value1", settings.Prop1);
        Assert.Equal(123, settings.Prop2);
    }

    [Fact]
    public void DeserializeSettings_CachesResultForSameType()
    {
        // Arrange
        var client = new AuthenticatedClient { Settings = "{\"Prop1\":\"Value1\"}" };

        // Act
        var settings1 = client.DeserializeSettings<TestSettings>();
        var settings2 = client.DeserializeSettings<TestSettings>();

        // Assert
        Assert.Same(settings1, settings2);
    }

    [Fact]
    public void DeserializeSettings_ReDeserializesForDifferentType()
    {
        // Arrange
        var client = new AuthenticatedClient { Settings = "{\"Prop1\":\"Value1\"}" };

        // Act
        var settings1 = client.DeserializeSettings<TestSettings>();
        var settings2 = client.DeserializeSettings<OtherSettings>();

        // Assert
        Assert.NotNull(settings1);
        Assert.NotNull(settings2);
        Assert.Equal("Value1", settings1.Prop1);
        Assert.Equal("Value1", settings2.Prop1);
    }

    [Fact]
    public void CreatePrincipal_ReturnsCorrectClaims()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientName = "Test Client";
        var client = new AuthenticatedClient { ID = clientId, Name = clientName };
        var scheme = "TestScheme";

        // Act
        var principal = client.CreatePrincipal(scheme);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal(scheme, principal.Identity?.AuthenticationType);
        Assert.True(principal.HasClaim(ClaimTypes.Sid, clientId.ToString()));
        Assert.True(principal.HasClaim(ClaimTypes.Name, clientName));
    }

    [Fact]
    public void DeserializeSettings_UsingExtension_ReturnsDeserializedObject()
    {
        // Arrange
        var client = new AuthenticatedClient { Settings = "{\"Prop1\":\"Value1\"}" };

        // Act
        var settings = client.DeserializeSettings<TestSettings>();

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("Value1", settings.Prop1);
    }

    private class TestSettings
    {
        public string Prop1 { get; set; } = default!;
        public int Prop2 { get; set; }
    }

    private class OtherSettings
    {
        public string Prop1 { get; set; } = default!;
    }
}
