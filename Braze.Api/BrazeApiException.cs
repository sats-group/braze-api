using System;
using System.Collections.Generic;
using System.Net;

namespace Braze.Api;

/// <summary>
/// An exception thrown by the Braze API.
/// </summary>
public class BrazeApiException(string message) : Exception(message)
{
    /// <summary>
    /// Minor error messages.
    /// </summary>
    public List<string>? Errors { get; init; }

    /// <summary>
    /// The <see cref="HttpStatusCode"/>, if this exception is a result of a fatal api response from Braze.
    /// </summary>
    public HttpStatusCode? HttpStatusCode { get; set; }

    /// <summary>
    /// X-Ratelimit-Retry-After: an integer indicating the number of seconds before you can start making requests.
    /// </summary>
    public int? RateLimitingRetryAfter { get; init; }
}

