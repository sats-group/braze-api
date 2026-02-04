using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Braze.Api;

/// <summary>
/// An exception thrown by the Braze API.
/// </summary>
public class BrazeApiException(string message) : Exception(message)
{
    /// <summary>
    /// Minor error messages.
    /// </summary>
    /// <remarks>The item type is subject to change, but no documentation on these error items have been found.</remarks>
    public List<JsonElement>? Errors { get; init; }

    /// <summary>
    /// The <see cref="HttpStatusCode"/>, if this exception is a result of a fatal api response from Braze.
    /// </summary>
    public HttpStatusCode? HttpStatusCode { get; set; }

    /// <summary>
    /// X-Ratelimit-Retry-After: an integer indicating the number of seconds before you can start making requests.
    /// </summary>
    public int? RateLimitingRetryAfter { get; init; }
}

