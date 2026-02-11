using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// The purchase request model.
/// </summary>
public class Purchase : BrazeUserIdentifier
{
    /// <summary>
    /// The app Id.
    /// </summary>
    [JsonPropertyName("app_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AppId { get; init; }

    /// <summary>
    /// The product Id.
    /// </summary>
    [JsonPropertyName("product_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string ProductId { get; init; }

    /// <summary>
    /// The currency.
    /// </summary>
    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string Currency { get; init; }

    /// <summary>
    /// The price.
    /// </summary>
    [JsonPropertyName("price")]
    public required decimal Price { get; init; }

    /// <summary>
    /// The quantity.
    /// </summary>
    [JsonPropertyName("quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Quantity { get; init; }

    /// <summary>
    /// The time.
    /// </summary>
    [JsonPropertyName("time")]
    public required DateTimeOffset Time { get; init; }

    /// <summary>
    /// The properties.
    /// </summary>
    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Property>? Properties { get; init; }

    /// <summary>
    /// If you have a user profile with a different external_id than the one in this request, you can use this to update the existing user profile.
    /// </summary>
    [JsonPropertyName("_update_existing_only")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UpdateExistingOnly { get; init; }
}
