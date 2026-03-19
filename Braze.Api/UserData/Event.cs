using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Braze.Api.UserData.ECommerce;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Braze.Api.UserData;

/// <summary>
/// The event request model.
/// </summary>
[JsonConverter(typeof(EventJsonConverter))]
public class Event : BrazeUserIdentifier
{
    /// <summary>
    /// The app id.
    /// </summary>
    [JsonPropertyName("app_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AppId { get; init; }

    /// <summary>
    /// The name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The time.
    /// </summary>
    [JsonPropertyName("time")]
    public required DateTimeOffset Time { get; init; }

    /// <summary>
    /// If you have a user profile with a different external_id than the one in this request, you can use this to update the existing user profile.
    /// </summary>
    [JsonPropertyName("_update_existing_only")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UpdateExistingOnly { get; init; }
}

/// <summary>
/// An ordinary Braze custom event, where the properties are defined in the event itself.
/// </summary>
public class CustomEvent : Event
{
    /// <summary>
    /// The properties.
    /// </summary>
    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? Properties { get; init; }
}
