using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// The subscription status set request model.
/// </summary>
public class SubscriptionStatusSetRequest
{
    /// <summary>
    /// The subscription groups to update.
    /// </summary>
    [JsonPropertyName("subscription_groups")]
    public required List<SubscriptionGroupUpdate> SubscriptionGroups { get; init; }
}
