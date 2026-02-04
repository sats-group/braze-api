using System.Net.Http;
using System.Net.Http.Json;
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
        var response = await responseMessage.Content.ReadFromJsonAsync<InternalApiResponse<T>>(cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var exceptionMessage =
                response?.Message
                ?? $"Unknown error while sending request to Braze: {responseMessage.RequestMessage?.Method} {responseMessage.RequestMessage?.RequestUri}.";

            throw new BrazeApiException(exceptionMessage)
            {
                HttpStatusCode = responseMessage.StatusCode,
                Errors = response?.Errors,
                RateLimitingRetryAfter = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Retry-After"),
            };
        }

        return new ApiResponse<T>(
            response?.Value,
            response?.Errors)
        {
            RateLimitingLimit = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Limit"),
            RateLimitingRemaining = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Remaining"),
            RateLimitingReset = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Reset"),
        };
    }
}
