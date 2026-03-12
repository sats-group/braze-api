using System.Collections.Generic;
using System.Text.Json.Serialization;
using Braze.Api.UserData;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Represents an individual recipient of a triggered campaign or canvas.
/// </summary>
public class Recipient
{
    /// <summary>
    /// User alias object.
    /// </summary>
    /// <remarks>
    /// You must include one of <see cref="ExternalUserId"/>, <see cref="UserAlias"/>, <see cref="BrazeId"/>,
    /// or <see cref="Email"/> in this object. Requests must specify only one.
    /// </remarks>
    [JsonPropertyName("user_alias")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UserAlias? UserAlias { get; init; }

    /// <summary>
    /// External ID of the user.
    /// </summary>
    /// <remarks>
    /// You must include one of <see cref="ExternalUserId"/>, <see cref="UserAlias"/>, <see cref="BrazeId"/>,
    /// or <see cref="Email"/> in this object. Requests must specify only one.
    /// </remarks>
    [JsonPropertyName("external_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ExternalUserId { get; init; }

    /// <summary>
    /// Braze identifier for the user.
    /// </summary>
    /// <remarks>
    /// You must include one of <see cref="ExternalUserId"/>, <see cref="UserAlias"/>, <see cref="BrazeId"/>,
    /// or <see cref="Email"/> in this object. Requests must specify only one.
    /// </remarks>
    [JsonPropertyName("braze_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BrazeId { get; init; }

    /// <summary>
    /// Email address of the user to receive the message.
    /// </summary>
    /// <remarks>
    /// You must include one of <see cref="ExternalUserId"/>, <see cref="UserAlias"/>, <see cref="BrazeId"/>,
    /// or <see cref="Email"/> in this object. Requests must specify only one.
    /// </remarks>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// Prioritization array.
    /// Required by the Braze API when <see cref="Email"/> is specified.
    /// </summary>
    [JsonPropertyName("prioritization")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Prioritization>? Prioritization { get; init; }

    /// <summary>
    /// Optional per-recipient trigger properties for the canvas entry.
    /// </summary>
    [JsonPropertyName("trigger_properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? TriggerProperties { get; init; }

    /// <summary>
    /// Optional per-user personalization key-value pairs.
    /// Overrides any keys that conflict with the parent <c>context</c>.
    /// </summary>
    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? Context { get; init; }

    /// <summary>
    /// When set to <c>true</c>, Braze will only send to existing users
    /// and will not create new user profiles from the request.
    /// Cannot be used with user aliases.
    /// When omitted, the Braze API defaults to <c>true</c> server-side.
    /// </summary>
    [JsonPropertyName("send_to_existing_only")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendToExistingOnly { get; init; }

    /// <summary>
    /// Optional user attributes to set before the canvas is triggered.
    /// Existing values are overwritten.
    /// </summary>
    [JsonPropertyName("attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UserAttribute? Attributes { get; init; }
}
