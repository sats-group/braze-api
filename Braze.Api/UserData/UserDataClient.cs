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
    Task<ApiResponse<TrackResponse>> Track(Track trackRequest, CancellationToken cancellationToken = default);
}

/// <inheritdoc/>
internal class UserDataClient(HttpClient httpClient) : IUserDataClient
{
    /// <inheritdoc/>
    public async Task<ApiResponse<TrackResponse>> Track(Track trackRequest, CancellationToken cancellationToken = default)
    {
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("users/track", UriKind.Relative))
        {
            Content = JsonContent.Create(trackRequest)
        };

        using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var errorResponse = await responseMessage.Content.ReadFromJsonAsync<ErrorApiResponse>(cancellationToken);

            throw new BrazeApiException(
                errorResponse?.Message
                ?? $"Unknown error while sending request to Braze: {requestMessage.Method} {requestMessage.RequestUri}.")
            {
                HttpStatusCode = responseMessage.StatusCode,
                Errors = errorResponse?.Errors,
                RateLimitingRetryAfter = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Retry-After"),
            };
        }

        var responseContent = await responseMessage.Content.ReadFromJsonAsync<InternalTrackResponseModel>(cancellationToken)
                              ?? throw new BrazeApiException("Unable to deserialize the response.");

        return new ApiResponse<TrackResponse>(
            new TrackResponse
            {
                AttributesProcessed = responseContent.AttributesProcessed,
                EventsProcessed = responseContent.EventsProcessed,
                PurchasesProcessed = responseContent.PurchasesProcessed,
            },
            responseContent.Errors)
        {
            RateLimitingLimit = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Limit"),
            RateLimitingRemaining = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Remaining"),
            RateLimitingReset = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Reset"),
        };
    }

    private class InternalTrackResponseModel : ApiResponseModel
    {
        [JsonPropertyName("attributes_processed")]
        public int? AttributesProcessed { get; init; }

        [JsonPropertyName("events_processed")]
        public int? EventsProcessed { get; init; }

        [JsonPropertyName("purchases_processed")]
        public int? PurchasesProcessed { get; init; }
    }
}

/// <summary>
/// The track request model.
/// </summary>
public class Track
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
    public required int? AttributesProcessed { get; init; }

    /// <summary>
    /// If events are included in the request, this returns an integer of the number of events that Braze queued for processing.
    /// </summary>
    [JsonPropertyName("events_processed")]
    public required int? EventsProcessed { get; init; }

    /// <summary>
    /// If purchases are included in the request, this returns an integer of the number of purchases that Braze queued for processing.
    /// </summary>
    [JsonPropertyName("purchases_processed")]
    public required int? PurchasesProcessed { get; init; }
}
