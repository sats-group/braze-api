using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api;

internal static class HttpResponseMessageExtensions
{
    public static async Task<ApiResponse<T>> CreateApiResponse<T>(
        this HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
        where T : class
    {
        InternalApiResponse<T>? response;

        if (!responseMessage.IsSuccessStatusCode)
        {
            var errorResponse =
                await responseMessage.Content.ReadFromJsonAsync<BrazeErrorResponse>(
                    DefaultJsonSerializerOptions.Options, cancellationToken);

            if (errorResponse is not null)
            {
                throw new BrazeApiException(errorResponse.Message ?? "Unknown error response returned from Braze")
                {
                    HttpStatusCode = responseMessage.StatusCode,
                    Errors = errorResponse?.Errors,
                    RateLimitingRetryAfter = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Retry-After"),
                };
            }

            var errorResponseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            throw new BrazeApiException(
                $"Unknown error response returned from Braze: {responseMessage.RequestMessage?.Method} {responseMessage.RequestMessage?.RequestUri}: {errorResponseBody}");

        }

        response = await GetResponseOrThrow<T>(responseMessage, cancellationToken);

        return new ApiResponse<T>(
            response?.Value,
            response?.Errors)
        {
            RateLimitingLimit = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Limit"),
            RateLimitingRemaining = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Remaining"),
            RateLimitingReset = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Reset"),
        };
    }

    private static async Task<InternalApiResponse<T>?> GetResponseOrThrow<T>(HttpResponseMessage responseMessage,
        CancellationToken cancellationToken) where T : class
    {
        InternalApiResponse<T>? response;
        try
        {
            response = await responseMessage.Content.ReadFromJsonAsync<InternalApiResponse<T>>(DefaultJsonSerializerOptions.Options, cancellationToken);
            if (response is null)
            {
                var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
                throw new BrazeApiException("Unable to parse response from Braze when calling " + responseMessage.RequestMessage?.RequestUri + ", got body: " + responseBody);
            }
        }
        catch (JsonException ex)
        {
            var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            throw new BrazeApiException("Unable to parse response from Braze when calling " + responseMessage.RequestMessage?.RequestUri + ", got body: " + responseBody, ex);
        }

        return response;
    }
}

internal class BrazeErrorResponse
{
    public string? Message { get; set; }
    public List<JsonElement>? Errors { get; init; }
}
