using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// A subscription state describing a user's subscription status in a subscription group.
/// </summary>
#if NET_9_0_OR_GREATER
[JsonConverter(typeof(JsonStringEnumConverter))]
#else
[JsonConverter(typeof(SubscriptionStateJsonConverter))]
#endif
public enum SubscriptionGroupSubscribeState
{
    /// <summary>
    /// Not in subscription group
    /// </summary>
    [JsonStringEnumMemberName("unsubscribed")]
    Unsubscribed = 0,
    /// <summary>
    /// In subscription group
    /// </summary>
    [JsonStringEnumMemberName("subscribed")]
    Subscribed
}
