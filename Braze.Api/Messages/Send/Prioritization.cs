using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// The <see cref="Prioritization"/> enum is used in 'prioritization' arrays to determine which user to merge if there are multiple users found.
/// 'prioritization' is an ordered array, meaning if more than one user matches from a prioritization, then merging does not occur.
/// </summary>
/// <remarks>
/// If an email address or phone number is specified as an identifier, you must also include prioritization in the identifier.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Prioritization
{
    /// <summary>
    /// <see cref="Identified"/> refers to prioritizing a user with an external_id.
    /// </summary>
    [JsonStringEnumMemberName("identified")]
    Identified = 1,

    /// <summary>
    /// <see cref="Unidentified"/> refers to prioritizing a user without an external_id.
    /// </summary>
    [JsonStringEnumMemberName("unidentified")]
    Unidentified = 2,

    /// <summary>
    /// <see cref="MostRecentlyUpdated"/> refers to prioritizing the most recently updated user.
    /// </summary>
    [JsonStringEnumMemberName("most_recently_updated")]
    MostRecentlyUpdated = 3,

    /// <summary>
    /// <see cref="LeastRecentlyUpdated"/> refers to prioritizing the least recently updated user.
    /// </summary>
    [JsonStringEnumMemberName("least_recently_updated")]
    LeastRecentlyUpdated = 4,
}
