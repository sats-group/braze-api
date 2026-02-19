using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api.UserData;

/// <summary>
/// A client for the user data endpoints.
/// </summary>
public interface IUserDataClient
{
    /// <summary>
    /// Tracks user data.
    /// </summary>
    /// <param name="trackRequest">The track request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The track response.</returns>
    Task<ApiResponse<TrackResponse>> Track(TrackRequest trackRequest, CancellationToken cancellationToken = default);
}

/// <inheritdoc/>
internal class UserDataClient(HttpClient httpClient) : IUserDataClient
{
    /// <inheritdoc/>
    public async Task<ApiResponse<TrackResponse>> Track(TrackRequest trackRequest, CancellationToken cancellationToken = default)
    {
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("users/track", UriKind.Relative))
        {
            Content = JsonContent.Create(trackRequest)
        };

        using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

        return await responseMessage.CreateApiResponse<TrackResponse>(cancellationToken);
    }
}

/// <summary>
/// The track request model.
/// </summary>
public class TrackRequest
{
    /// <summary>
    /// The attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<UserAttribute>? Attributes { get; init; }

    /// <summary>
    /// The events.
    /// </summary>
    [JsonPropertyName("events")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Event>? Events { get; init; }

    /// <summary>
    /// The purchases.
    /// </summary>
    [JsonPropertyName("purchases")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Purchase>? Purchases { get; init; }
}

/// <summary>
/// The track response.
/// </summary>
public class TrackResponse
{
    /// <summary>
    /// If attributes are included in the request, this returns an integer of the number of external_ids with attributes that Braze queued for processing.
    /// </summary>
    [JsonPropertyName("attributes_processed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? AttributesProcessed { get; init; }

    /// <summary>
    /// If events are included in the request, this returns an integer of the number of events that Braze queued for processing.
    /// </summary>
    [JsonPropertyName("events_processed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EventsProcessed { get; init; }

    /// <summary>
    /// If purchases are included in the request, this returns an integer of the number of purchases that Braze queued for processing.
    /// </summary>
    [JsonPropertyName("purchases_processed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PurchasesProcessed { get; init; }
}
