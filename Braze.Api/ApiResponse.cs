using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Braze.Api;

/// <summary>
/// Response from an api call.
/// </summary>
/// <typeparam name="T">The type of the value returned from the api call.</typeparam>
public class ApiResponse<T>
{
    internal ApiResponse(T? value, List<JsonElement>? nonFatalErrors)
    {
        Value = value;
        NonFatalErrors = nonFatalErrors;
        Success = value is not null
                  && (nonFatalErrors is null
                      || nonFatalErrors.Count == 0);
    }

    /// <summary>
    /// Creates a successful API response for testing purposes.
    /// </summary>
    /// <param name="value">The response value.</param>
    /// <param name="rateLimitingLimit">The rate limit (default: 250000).</param>
    /// <param name="rateLimitingRemaining">The remaining rate limit (default: 249999).</param>
    /// <param name="rateLimitingReset">The rate limit reset time in seconds (default: 60).</param>
    /// <returns>A successful API response.</returns>
    public static ApiResponse<T> CreateSuccess(
        T value,
        int rateLimitingLimit = 250000,
        int rateLimitingRemaining = 249999,
        int rateLimitingReset = 60) =>
        new(value, null)
        {
            RateLimitingLimit = rateLimitingLimit,
            RateLimitingRemaining = rateLimitingRemaining,
            RateLimitingReset = rateLimitingReset,
        };

    /// <summary>
    /// Creates an API response with non-fatal errors for testing purposes.
    /// </summary>
    /// <param name="value">The response value (can be null).</param>
    /// <param name="nonFatalErrors">The list of non-fatal errors.</param>
    /// <param name="rateLimitingLimit">The rate limit (default: 250000).</param>
    /// <param name="rateLimitingRemaining">The remaining rate limit (default: 249999).</param>
    /// <param name="rateLimitingReset">The rate limit reset time in seconds (default: 60).</param>
    /// <returns>An API response with non-fatal errors.</returns>
    public static ApiResponse<T> CreateWithErrors(
        T? value,
        List<JsonElement> nonFatalErrors,
        int rateLimitingLimit = 250000,
        int rateLimitingRemaining = 249999,
        int rateLimitingReset = 60) =>
        new(value, nonFatalErrors)
        {
            RateLimitingLimit = rateLimitingLimit,
            RateLimitingRemaining = rateLimitingRemaining,
            RateLimitingReset = rateLimitingReset,
        };

    /// <summary>
    /// Indicates that the api call was successful and had no non-fatal errors (fatal errors will throw).
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
    /// <remarks>The item type is subject to change, but no documentation on these error items have been found.</remarks>
    public List<JsonElement>? NonFatalErrors { get; }
}
