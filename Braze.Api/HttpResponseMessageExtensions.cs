using System.Net.Http;
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
        var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        if (!responseMessage.IsSuccessStatusCode)
        {
            BrazeErrorResponse? errorResponse;
            try
            {
                errorResponse =
                    JsonSerializer.Deserialize<BrazeErrorResponse>(responseBody, DefaultJsonSerializerOptions.Options);
            }
            catch (JsonException)
            {
                errorResponse = null;
            }

            if (errorResponse is not null)
            {
                throw new BrazeApiException(errorResponse.Message ?? "Unknown error response returned from Braze")
                {
                    HttpStatusCode = responseMessage.StatusCode,
                    Errors = errorResponse.Errors,
                    RateLimitingRetryAfter = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Retry-After"),
                    Endpoint = responseMessage.RequestMessage?.RequestUri
                };
            }

            throw new BrazeApiException(
                $"Unknown error response returned from Braze: {responseMessage.RequestMessage?.Method} {responseMessage.RequestMessage?.RequestUri}: {responseBody}")
            {
                Endpoint = responseMessage.RequestMessage?.RequestUri
            };
        }

        var response = GetResponseOrThrow<T>(responseMessage, responseBody);

        return new ApiResponse<T>(
            response.Value,
            response.Errors)
        {
            RateLimitingLimit = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Limit"),
            RateLimitingRemaining = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Remaining"),
            RateLimitingReset = responseMessage.Headers.GetIntOrDefault("X-RateLimit-Reset"),
        };
    }

    private static InternalApiResponse<T> GetResponseOrThrow<T>(
        HttpResponseMessage responseMessage,
        string responseBody) where T : class
    {
        InternalApiResponse<T>? response;
        try
        {
            response = JsonSerializer.Deserialize<InternalApiResponse<T>>(responseBody, DefaultJsonSerializerOptions.Options);
            if (response is null)
            {
                throw new BrazeApiException("Unable to parse response from Braze when calling " +
                                            responseMessage.RequestMessage?.RequestUri + ", got body: " + responseBody)
                {
                    Endpoint = responseMessage.RequestMessage?.RequestUri
                };
            }
        }
        catch (JsonException ex)
        {
            throw new BrazeApiException(
                "Unable to parse response from Braze when calling " + responseMessage.RequestMessage?.RequestUri +
                ", got body: " + responseBody, ex)
            {
                Endpoint = responseMessage.RequestMessage?.RequestUri
            };
        }

        return response;
    }
}
