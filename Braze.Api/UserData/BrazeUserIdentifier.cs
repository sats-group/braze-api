using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// The user identifier request model.
/// </summary>
public class BrazeUserIdentifier
{
    /// <summary>
    /// The external Id.
    /// </summary>
    [JsonPropertyName("external_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ExternalId { get; init; }

    /// <summary>
    /// The user alias.
    /// </summary>
    [JsonPropertyName("user_alias")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UserAlias? UserAlias { get; init; }

    /// <summary>
    /// The braze Id.
    /// </summary>
    [JsonPropertyName("braze_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BrazeId { get; init; }

    /// <summary>
    /// The email.
    /// </summary>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// The phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; init; }
}

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
