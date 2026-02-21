using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// The subscription status set response.
/// </summary>
public class SubscriptionStatusSetResponse
{
    /// <summary>
    /// A message describing the result of the operation.
    /// </summary>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }
}
