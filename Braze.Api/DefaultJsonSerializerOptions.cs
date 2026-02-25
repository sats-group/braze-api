using System.Text.Json;

namespace Braze.Api;

/// <summary>
/// JsonSerializerOptions used by the Braze API.
/// </summary>
public static class DefaultJsonSerializerOptions
{
    /// <summary>
    /// Use these options when serializing/deserializing Braze API models.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        RespectNullableAnnotations = true,
    };
}
