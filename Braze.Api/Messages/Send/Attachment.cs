using System;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Represents a file attachment for email campaigns.
/// See https://www.braze.com/docs/api/endpoints/messaging/send_messages/post_send_triggered_campaigns
/// </summary>
public class Attachment
{
    /// <summary>
    /// The name of the file you want to attach to your email, excluding the extension (e.g., ".pdf").
    /// Attach files up to 2 MB.
    /// </summary>
    [JsonPropertyName("file_name")]
    public required string FileName { get; init; }

    /// <summary>
    /// The corresponding URL of the file you want to attach to your email.
    /// The file name's extension is detected automatically from the URL defined.
    /// </summary>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }
}
