using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Request body for sending a triggered campaign message.
/// Braze endpoint: POST /campaigns/trigger/send
/// </summary>
public class TriggeredCampaign
{
    /// <summary>
    /// The ID of the campaign to trigger.
    /// </summary>
    [JsonPropertyName("campaign_id")]
    public required string CampaignId { get; init; }

    /// <summary>
    /// The sender ID of the message, see https://www.braze.com/docs/api/identifier_types/#send-identifier.
    /// </summary>
    [JsonPropertyName("send_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SendId { get; init; }

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

    /// <summary>
    /// Optional list of recipients.
    /// </summary>
    [JsonPropertyName("recipients")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Recipient>? Recipients { get; init; }

    /// <summary>
    /// Attachments to send with the message.
    /// </summary>
    [JsonPropertyName("attachments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Attachment>? Attachments { get; init; }
}
