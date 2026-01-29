using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Braze.Api;

/// <summary>
/// Response from an api call.
/// </summary>
/// <typeparam name="T">The type of the value returned from the api call.</typeparam>
public class ApiResponse<T>
{
    internal ApiResponse(T value)
    {
        Success = true;
        Value = value;
    }

    /// <summary>
    /// Indicates that the api call was successful or had a non-fatal response (fatal errors will throw).
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool Success { get; }

    /// <summary>
    /// The value returned from the api call.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// X-RateLimit-Limit: The number of requests allowed per time period
    /// </summary>
    public required int RateLimitingLimit { get; init; }

    /// <summary>
    /// X-RateLimit-Remaining: The approximate number of requests remaining within a window
    /// </summary>
    public required int RateLimitingRemaining { get; init; }

    /// <summary>
    /// X-RateLimit-Reset: The number of seconds remaining before the current window resets
    /// </summary>
    public required int RateLimitingReset { get; init; }

    /// <summary>
    /// A list of non-fatal errors.
    /// </summary>
    public List<string>? NonFatalErrors { get; init; }
}
