using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Represents an individual recipient of the triggered campaign.
/// </summary>
public class TriggeredCampaignRecipient
{
    /// <summary>
    /// External ID of the user.
    /// </summary>
    [JsonPropertyName("external_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ExternalUserId { get; init; }

    /// <summary>
    /// User alias object.
    /// </summary>
    [JsonPropertyName("user_alias")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UserAlias? UserAlias { get; init; }

    /// <summary>
    /// Optional per-user trigger properties.
    /// Overrides global trigger_properties.
    /// </summary>
    [JsonPropertyName("trigger_properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? TriggerProperties { get; init; }
}
