using System.Text.Json.Serialization;

namespace Braze.Api;

/// <summary>
/// https://www.braze.com/docs/api/objects_filters/user_alias_object
/// </summary>
public class UserAlias
{
    /// <summary>
    /// An alias_name for the identifier itself.
    /// </summary>
    [JsonPropertyName("alias_name")]
    public required string Name { get; init; }

    /// <summary>
    /// An alias_label indicating the type of alias.
    /// </summary>
    [JsonPropertyName("alias_label")]
    public required string Label { get; init; }
}
