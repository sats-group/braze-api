using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// A subscription state describing a user's subscription status in a subscription group.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
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
