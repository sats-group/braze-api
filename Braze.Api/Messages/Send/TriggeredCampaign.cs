using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Request body for sending a triggered campaign message.
/// Braze endpoint: POST /messages/send/triggered/campaigns
/// </summary>
public class TriggeredCampaign
{
    /// <summary>
    /// The ID of the campaign to trigger.
    /// </summary>
    [JsonPropertyName("campaign_id")]
    public required string CampaignId { get; init; }

    /// <summary>
    /// Optional ID of the message variation to send.
    /// If omitted, Braze will choose the default variation.
    /// </summary>
    [JsonPropertyName("campaign_variation_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CampaignVariationId { get; init; }

    /// <summary>
    /// Optional audience definition for the request.
    /// </summary>
    [JsonPropertyName("audience")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TriggeredCampaignAudience? Audience { get; init; }

    /// <summary>
    /// Optional list of recipients.
    /// </summary>
    [JsonPropertyName("recipients")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TriggeredCampaignRecipient>? Recipients { get; init; }

    /// <summary>
    /// Optional custom properties passed into the campaign.
    /// These can be referenced in Liquid templates.
    /// </summary>
    [JsonPropertyName("trigger_properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? TriggerProperties { get; init; }

    /// <summary>
    /// Optional flag to send this request as a broadcast.
    /// </summary>
    [JsonPropertyName("broadcast")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Broadcast { get; init; }
}
