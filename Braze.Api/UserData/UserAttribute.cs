using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// The user attribute request model.
/// </summary>
public class UserAttribute
{
    /// <summary>
    /// The external Id.
    /// </summary>
    [JsonPropertyName("external_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ExternalId { get; init; }

    /// <summary>
    /// The user alias.
    /// </summary>
    [JsonPropertyName("user_alias")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UserAlias? UserAlias { get; init; }

    /// <summary>
    /// The braze Id.
    /// </summary>
    [JsonPropertyName("braze_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BrazeId { get; init; }

    /// <summary>
    /// The email.
    /// </summary>
    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; init; }

    /// <summary>
    /// The phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; init; }

    /// <summary>
    /// If you have a user profile with a different external_id than the one in this request, you can use this to update the existing user profile.
    /// </summary>
    [JsonPropertyName("_update_existing_only")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UpdateExistingOnly { get; init; }

    /// <summary>
    /// If you are updating a user profile that has a push token, you can use this to import the push token.
    /// </summary>
    [JsonPropertyName("push_token_import")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? PushTokenImport { get; init; }

    /// <summary>
    /// The country.
    /// </summary>
    [JsonPropertyName("country")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Country { get; init; }

    /// <summary>
    /// The current location.
    /// </summary>
    [JsonPropertyName("current_location")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Location? CurrentLocation { get; init; }

    /// <summary>
    /// The date of the first session.
    /// </summary>
    [JsonPropertyName("date_of_first_session")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? DateOfFirstSession { get; init; }

    /// <summary>
    /// The date of the last session.
    /// </summary>
    [JsonPropertyName("date_of_last_session")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? DateOfLastSession { get; init; }

    /// <summary>
    /// The date of birth.
    /// </summary>
    [JsonPropertyName("dob")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? DateOfBirth { get; init; }

    /// <summary>
    /// The email subscription state.
    /// </summary>
    [JsonPropertyName("email_subscribe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SubscribeState? EmailSubscribe { get; init; }

    /// <summary>
    /// Disables email open tracking.
    /// </summary>
    [JsonPropertyName("email_open_tracking_disabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? EmailOpenTrackingDisabled { get; init; }

    /// <summary>
    /// Disables email click tracking.
    /// </summary>
    [JsonPropertyName("email_click_tracking_disabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? EmailClickTrackingDisabled { get; init; }

    /// <summary>
    /// The facebook data.
    /// </summary>
    [JsonPropertyName("facebook")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FacebookTrackRequestModel? Facebook { get; init; }

    /// <summary>
    /// The gender.
    /// </summary>
    [JsonPropertyName("gender")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Gender? Gender { get; init; }

    /// <summary>
    /// The home city.
    /// </summary>
    [JsonPropertyName("home_city")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HomeCity { get; init; }

    /// <summary>
    /// The language.
    /// </summary>
    [JsonPropertyName("language")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Language { get; init; }

    /// <summary>
    /// The last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; init; }

    /// <summary>
    /// The date the user marked the email as spam.
    /// </summary>
    [JsonPropertyName("marked_email_as_spam_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MarkedEmailAsSpamAt { get; init; }

    /// <summary>
    /// The push subscription state.
    /// </summary>
    [JsonPropertyName("push_subscribe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SubscribeState? PushSubscribe { get; init; }

    /// <summary>
    /// The push tokens.
    /// </summary>
    [JsonPropertyName("push_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PushToken>? PushTokens { get; init; }

    /// <summary>
    /// The subscription groups.
    /// </summary>
    [JsonPropertyName("subscription_groups")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SubscriptionGroup>? SubscriptionGroups { get; init; }

    /// <summary>
    /// The time zone.
    /// </summary>
    [JsonPropertyName("time_zone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TimeZone { get; init; }

    /// <summary>
    /// The twitter data.
    /// </summary>
    [JsonPropertyName("twitter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TwitterTrackRequestModel? Twitter { get; init; }

    /// <summary>
    /// The custom attributes.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, PropertyOp>? CustomAttributes { get; init; }

    [JsonExtensionData]
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#pragma warning disable IDE0051
    private Dictionary<string, object>? CustomAttributesExtensionData => CustomAttributes?
#pragma warning restore IDE0051
        .ToDictionary(
            kv => kv.Key,
            kv => (object)kv.Value);
}

/// <summary>
/// The facebook track request model.
/// </summary>
public class FacebookTrackRequestModel
{
    /// <summary>
    /// The Id.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; init; }

    /// <summary>
    /// The likes.
    /// </summary>
    [JsonPropertyName("likes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Likes { get; init; }

    /// <summary>
    /// The number of friends.
    /// </summary>
    [JsonPropertyName("num_friends")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumFriends { get; init; }
}

/// <summary>
/// The twitter track request model.
/// </summary>
public class TwitterTrackRequestModel
{
    /// <summary>
    /// The Id.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; init; }

    /// <summary>
    /// The screen name.
    /// </summary>
    [JsonPropertyName("screen_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ScreenName { get; init; }

    /// <summary>
    /// The followers count.
    /// </summary>
    [JsonPropertyName("followers_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? FollowersCount { get; init; }

    /// <summary>
    /// The friends count.
    /// </summary>
    [JsonPropertyName("friends_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? FriendsCount { get; init; }

    /// <summary>
    /// The statuses count.
    /// </summary>
    [JsonPropertyName("statuses_count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StatusesCount { get; init; }
}

/// <summary>
/// The subscription group.
/// </summary>
public class SubscriptionGroup
{
    /// <summary>
    /// The Id.
    /// </summary>
    [JsonPropertyName("subscription_group_id")]
    public required string Id { get; init; }

    /// <summary>
    /// The subscription state.
    /// </summary>
    [JsonPropertyName("subscription_state")]
    public required SubscribeGroupState State { get; init; }
}

/// <summary>
/// The subscribe group state.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscribeGroupState
{
    /// <summary>
    /// Subscribed.
    /// </summary>
    [JsonStringEnumMemberName("subscribed")]
    Subscribed = 1,

    /// <summary>
    /// Unsubscribed.
    /// </summary>
    [JsonStringEnumMemberName("unsubscribed")]
    Unsubscribed = 2,
}

/// <summary>
/// The push token.
/// </summary>
public class PushToken
{
    /// <summary>
    /// The app Id.
    /// </summary>
    [JsonPropertyName("app_id")]
    public required string AppId { get; init; }

    /// <summary>
    /// The token.
    /// </summary>
    [JsonPropertyName("token")]
    public required string Token { get; init; }

    /// <summary>
    /// The device Id.
    /// </summary>
    [JsonPropertyName("device_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DeviceId { get; init; }
}

/// <summary>
/// Gender enum.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    /// <summary>
    /// Unknown
    /// </summary>
    [JsonStringEnumMemberName("nil")]
    Unknown = 0,

    /// <summary>
    /// Male
    /// </summary>
    [JsonStringEnumMemberName("M")]
    Male = 1,

    /// <summary>
    /// Female
    /// </summary>
    [JsonStringEnumMemberName("F")]
    Female = 2,

    /// <summary>
    /// Other
    /// </summary>
    [JsonStringEnumMemberName("O")]
    Other = 3,

    /// <summary>
    /// Prefer not to say.
    /// </summary>
    [JsonStringEnumMemberName("P")]
    PreferNotToSay = 4,
}

/// <summary>
/// The subscribe state.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscribeState
{
    /// <summary>
    /// Explicitly registered to receive email messages.
    /// </summary>
    [JsonStringEnumMemberName("opted_in")]
    OptedIn = 1,

    /// <summary>
    /// Explicitly opted out of email messages.
    /// </summary>
    [JsonStringEnumMemberName("unsubscribed")]
    Unsubscribed = 2,

    /// <summary>
    /// Neither opted in nor out.
    /// </summary>
    [JsonStringEnumMemberName("subscribed")]
    Subscribed = 3,
}

/// <summary>
/// A geo location.
/// </summary>
public class Location
{
    /// <summary>
    /// The latitude part of this <see cref="Location"/>.
    /// </summary>
    [JsonPropertyName("latitude")]
    public required double Latitude { get; init; }

    /// <summary>
    /// The longitude part of this <see cref="Location"/>.
    /// </summary>
    [JsonPropertyName("longitude")]
    public required double Longitude { get; init; }
}
