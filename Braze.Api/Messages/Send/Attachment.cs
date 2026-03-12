using System;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Attachment to send with a Braze message, typically via an API-triggered campaign.
/// </summary>
public class Attachment
{
    /// <summary>
    /// The file name of the attachment.
    /// </summary>
    [JsonPropertyName("file_name")]
    public required string FileName { get; init; }

    /// <summary>
    /// The Uri of the file to attach.
    /// </summary>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }
}
