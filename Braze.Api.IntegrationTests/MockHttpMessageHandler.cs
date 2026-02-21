using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// A mock HTTP message handler that captures the request and returns a configured response.
/// </summary>
internal class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<ConfiguredResponse> _responses = new();
    private readonly List<HttpRequestMessage> _capturedRequests = new();

    /// <summary>
    /// Gets all captured requests.
    /// </summary>
    public IReadOnlyList<HttpRequestMessage> CapturedRequests => _capturedRequests.AsReadOnly();

    /// <summary>
    /// Gets the last captured request.
    /// </summary>
    public HttpRequestMessage? LastRequest => _capturedRequests.Count > 0 ? _capturedRequests[^1] : null;

    /// <summary>
    /// Configure the next response to return.
    /// </summary>
    public void ConfigureResponse(
        HttpStatusCode statusCode,
        string? content = null,
        Dictionary<string, string>? headers = null)
    {
        _responses.Enqueue(new ConfiguredResponse
        {
            StatusCode = statusCode,
            Content = content,
            Headers = headers ?? new Dictionary<string, string>(),
        });
    }

    /// <summary>
    /// Configure a success response with rate limiting headers.
    /// </summary>
    public void ConfigureSuccessResponse(string content, int rateLimit = 10000, int rateLimitRemaining = 9999, int rateLimitReset = 60)
    {
        ConfigureResponse(HttpStatusCode.OK, content, new Dictionary<string, string>
        {
            { "X-RateLimit-Limit", rateLimit.ToString() },
            { "X-RateLimit-Remaining", rateLimitRemaining.ToString() },
            { "X-RateLimit-Reset", rateLimitReset.ToString() }
        });
    }

    /// <summary>
    /// Configure a rate limited response.
    /// </summary>
    public void ConfigureRateLimitedResponse(string content, int retryAfter = 5)
    {
        ConfigureResponse(HttpStatusCode.TooManyRequests, content, new Dictionary<string, string>
        {
            { "X-RateLimit-Retry-After", retryAfter.ToString() }
        });
    }

    /// <summary>
    /// Reset the handler, clearing all captured requests and configured responses.
    /// </summary>
    public void Reset()
    {
        _capturedRequests.Clear();
        _responses.Clear();
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _capturedRequests.Add(request);

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException(
                "No response configured. Use ConfigureResponse() before making requests.");
        }

        var configuredResponse = _responses.Dequeue();

        var response = new HttpResponseMessage(configuredResponse.StatusCode)
        {
            RequestMessage = request
        };

        if (configuredResponse.Content != null)
        {
            response.Content = new StringContent(configuredResponse.Content, System.Text.Encoding.UTF8, "application/json");
        }

        foreach (var header in configuredResponse.Headers)
        {
            response.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return Task.FromResult(response);
    }

    private class ConfiguredResponse
    {
        public required HttpStatusCode StatusCode { get; init; }
        public string? Content { get; init; }
        public required Dictionary<string, string> Headers { get; init; }
        public HttpRequestMessage? Request { get; init; }
    }
}
