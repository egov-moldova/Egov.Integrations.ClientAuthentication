using System.Text.Json;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Extension methods for <see cref="AuthenticatedClient"/>.
/// </summary>
public static class AuthenticatedClientExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Deserializes JSON settings for an <see cref="AuthenticatedClient"/>.
    /// Note that settings property names are treated as case-insensitive.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings to deserialize.</typeparam>
    /// <param name="client">The client that includes the settings to be deserialized.</param>
    /// <returns>An instance of deserialized settings, if non-null.</returns>
    public static TSettings? DeserializeSettings<TSettings>(this AuthenticatedClient client)
    {
        if (client.Settings == null) return default;
        return JsonSerializer.Deserialize<TSettings>(client.Settings, JsonSerializerOptions);
    }
}
