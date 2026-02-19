using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Defines the audience for a triggered campaign send.
/// </summary>
public class TriggeredCampaignAudience
{
    /// <summary>
    /// External user IDs to target.
    /// </summary>
    [JsonPropertyName("external_user_ids")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? ExternalUserIds { get; init; }

    /// <summary>
    /// User aliases to target.
    /// </summary>
    [JsonPropertyName("aliases")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<UserAlias>? Aliases { get; init; }

    /// <summary>
    /// Segment ID to target.
    /// </summary>
    [JsonPropertyName("segment_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SegmentId { get; init; }
}
