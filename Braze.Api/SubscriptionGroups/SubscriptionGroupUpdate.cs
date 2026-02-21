using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// Represents a subscription group update.
/// </summary>
public class SubscriptionGroupUpdate
{
    /// <summary>
    /// The ID of the subscription group.
    /// </summary>
    [JsonConverter(typeof(GuidToLowerCaseStringConverter))]
    public required Guid SubscriptionGroupId { get; init; }

    /// <summary>
    /// The subscription state. Available values are "unsubscribed" (not in subscription group) or "subscribed" (in subscription group).
    /// </summary>
    public required SubscriptionGroupSubscribeState SubscriptionState { get; init; }

    /// <summary>
    /// The external IDs of the users. The total number of users across all identifier types (external_ids, emails, phones) must not exceed 50
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? ExternalIds { get; init; }

    /// <summary>
    /// The email addresses of the users. Must include at least one email address (with a maximum of 50) when identifying users by email.
    /// The total number of users across all identifier types (external_ids, emails, phones) must not exceed 50
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Emails { get; init; }

    /// <summary>
    /// The phone numbers of the users in E.164 format. Must include at least one phone number (up to 50) when identifying users by phone number.
    /// The total number of users across all identifier types (external_ids, emails, phones) must not exceed 50
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Phones { get; init; }

    /// <summary>
    /// If this parameter is omitted or set to false, users are not entered into the SMS double opt-in workflow.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UseDoubleOptInLogic { get; init; }
}
