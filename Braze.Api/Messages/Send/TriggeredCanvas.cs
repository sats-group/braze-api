using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Request body for sending a triggered canvas message.
/// Braze endpoint: POST /canvas/trigger/send
/// </summary>
public class TriggeredCanvas
{
    /// <summary>
    /// The ID of the canvas to trigger.
    /// </summary>
    [JsonPropertyName("canvas_id")]
    public required string CanvasId { get; init; }

    /// <summary>
    /// Optional personalization key-value pairs that apply to all users in this request.
    /// These can be referenced in Liquid templates.
    /// </summary>
    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? Context { get; init; }

    /// <summary>
    /// Optional flag to send this request as a broadcast.
    /// Must be set to <c>true</c> if <see cref="Recipients"/> is omitted.
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
}
