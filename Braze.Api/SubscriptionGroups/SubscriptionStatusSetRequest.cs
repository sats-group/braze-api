using System.Collections.Generic;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// The subscription status set request model.
/// </summary>
public class SubscriptionStatusSetRequest
{
    /// <summary>
    /// The subscription groups to update.
    /// </summary>
    public required List<SubscriptionGroupUpdate> SubscriptionGroups { get; init; }
}
