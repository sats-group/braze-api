using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// A client for the subscription groups endpoints.
/// </summary>
public interface ISubscriptionGroupsClient
{
    /// <summary>
    /// Updates the subscription status for up to 50 users.
    /// </summary>
    /// <param name="request">The subscription status update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The API response.</returns>
    Task<ApiResponse<SubscriptionStatusSetResponse>> SetSubscriptionStatus(SubscriptionStatusSetRequest request, CancellationToken cancellationToken = default);
}

/// <inheritdoc/>
internal class SubscriptionGroupsClient(HttpClient httpClient) : ISubscriptionGroupsClient
{
    /// <inheritdoc/>
    public async Task<ApiResponse<SubscriptionStatusSetResponse>> SetSubscriptionStatus(SubscriptionStatusSetRequest request, CancellationToken cancellationToken = default)
    {
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("v2/subscription/status/set", UriKind.Relative))
        {
            Content = JsonContent.Create(request)
        };

        using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

        return await responseMessage.CreateApiResponse<SubscriptionStatusSetResponse>(cancellationToken);
    }
}

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

/// <summary>
/// Represents a subscription group update.
/// </summary>
public class SubscriptionGroupUpdate
{
    /// <summary>
    /// The ID of the subscription group.
    /// </summary>
    [JsonPropertyName("subscription_group_id")]
    public required string SubscriptionGroupId { get; init; }

    /// <summary>
    /// The subscription state. Available values are "unsubscribed" (not in subscription group) or "subscribed" (in subscription group).
    /// </summary>
    [JsonPropertyName("subscription_state")]
    public required SubscriptionState SubscriptionState { get; init; }

    /// <summary>
    /// The external IDs of the users. The total number of users across all identifier types (external_ids, emails, phones) must not exceed 50
    /// </summary>
    [JsonPropertyName("external_ids")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? ExternalIds { get; init; }

    /// <summary>
    /// The email addresses of the users. Must include at least one email address (with a maximum of 50) when identifying users by email.
    /// The total number of users across all identifier types (external_ids, emails, phones) must not exceed 50
    /// </summary>
    [JsonPropertyName("emails")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Emails { get; init; }

    /// <summary>
    /// The phone numbers of the users in E.164 format. Must include at least one phone number (up to 50) when identifying users by phone number.
    /// The total number of users across all identifier types (external_ids, emails, phones) must not exceed 50
    /// </summary>
    [JsonPropertyName("phones")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Phones { get; init; }

    /// <summary>
    /// If this parameter is omitted or set to false, users are not entered into the SMS double opt-in workflow.
    /// </summary>
    [JsonPropertyName("use_double_opt_in_logic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UseDoubleOptInLogic { get; init; }
}

/// <summary>
/// A subscription state describing a user's subscription status in a subscription group.
/// </summary>
#if NET_9_0_OR_GREATER
[JsonConverter(typeof(JsonStringEnumConverter))]
#else
[JsonConverter(typeof(SubscriptionStateJsonConverter))]
#endif
public enum SubscriptionState
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

internal class SubscriptionStateJsonConverter : JsonConverter<SubscriptionState>
{
    /// <summary>
    /// /// <inheritdoc/>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override SubscriptionState Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Enum.Parse<SubscriptionState>(reader.GetString() ?? string.Empty, true);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="dateTimeValue"></param>
    /// <param name="options"></param>
    public override void Write(
        Utf8JsonWriter writer,
        SubscriptionState dateTimeValue,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(dateTimeValue.ToString().ToLowerInvariant());
}
