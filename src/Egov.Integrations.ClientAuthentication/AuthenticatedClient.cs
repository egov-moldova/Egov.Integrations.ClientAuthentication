using System.Security.Claims;
using System.Text.Json;

namespace Egov.Integrations.ClientAuthentication;

/// <summary>
/// Represents information about an authenticated client system.
/// </summary>
public class AuthenticatedClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Client system identifier.
    /// </summary>
    public Guid ID { get; init; }

    /// <summary>
    /// Client system name.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Client system owner identifier.
    /// </summary>
    public string? OwnerID { get; init; }

    /// <summary>
    /// A list explicitly configured certificates for the client.
    /// </summary>
    public List<byte[]>? Certificates { get; init; }

    /// <summary>
    /// Client system settings.
    /// </summary>
    public string? Settings { get; init; }

    private Type? _deserializedSettingsType;
    private object? _deserializedSettings;

    /// <summary>
    /// Deserializes settings into a JSON.
    /// Settings property names are treated as case-insensitive.
    /// Note that deserialized settings instance is cached for the same type of <typeparamref name="TSettings"/>.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings to deserialize.</typeparam>
    /// <returns>Deserialized settings, if non-null.</returns>
    public TSettings? DeserializeSettings<TSettings>()
    {
        if (Settings == null) return default;
        if (_deserializedSettingsType == typeof(TSettings)) return (TSettings?) _deserializedSettings;

        var deserializedSettings = JsonSerializer.Deserialize<TSettings>(Settings, SerializerOptions);
        _deserializedSettings = deserializedSettings;
        _deserializedSettingsType = typeof(TSettings);
        return deserializedSettings;
    }

    internal ClaimsPrincipal CreatePrincipal(string authenticationScheme)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Sid, ID.ToString()),
            new Claim(ClaimTypes.Name, Name)
        }, authenticationScheme));
    }
}
