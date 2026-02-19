using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.Tests;

/// <summary>
/// Example tests demonstrating how to create ApiResponse objects for unit testing.
/// These factory methods make it easy to test code that depends on API responses
/// without making actual API calls.
/// </summary>
public class MockingExampleTests
{
    [Fact]
    public void CreateSuccess_ReturnsSuccessfulResponse()
    {
        // Arrange & Act: Create a successful response using the factory method
        var response = ApiResponse<TrackResponse>.CreateSuccess(
            new TrackResponse
            {
                AttributesProcessed = 1,
                EventsProcessed = 2,
                PurchasesProcessed = 3
            });

        // Assert: Verify the response properties
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Equal(1, response.Value.AttributesProcessed);
        Assert.Equal(2, response.Value.EventsProcessed);
        Assert.Equal(3, response.Value.PurchasesProcessed);
        Assert.Equal(250000, response.RateLimitingLimit);
        Assert.Equal(249999, response.RateLimitingRemaining);
        Assert.Equal(60, response.RateLimitingReset);
    }

    [Fact]
    public void CreateSuccess_WithCustomRateLimits()
    {
        // Arrange & Act: Create a response with custom rate limits
        var response = ApiResponse<TrackResponse>.CreateSuccess(
            new TrackResponse
            {
                AttributesProcessed = 5
            },
            rateLimitingLimit: 100000,
            rateLimitingRemaining: 50000,
            rateLimitingReset: 120);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(5, response.Value.AttributesProcessed);
        Assert.Equal(100000, response.RateLimitingLimit);
        Assert.Equal(50000, response.RateLimitingRemaining);
        Assert.Equal(120, response.RateLimitingReset);
    }

    [Fact]
    public void CreateWithErrors_ReturnsFailedResponse()
    {
        // Arrange & Act: Create a response with non-fatal errors
        var response = ApiResponse<TrackResponse>.CreateWithErrors(
            new TrackResponse
            {
                AttributesProcessed = 0
            },
            [System.Text.Json.JsonDocument.Parse("{\"error\":\"test\"}").RootElement]);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Value);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Single(response.NonFatalErrors);
    }

    [Fact]
    public void CreateWithErrors_NullValue_ReturnsFailedResponse()
    {
        // Arrange & Act: Create a response with null value and errors
        var response = ApiResponse<TrackResponse>.CreateWithErrors(
            null,
            [System.Text.Json.JsonDocument.Parse("{\"error\":\"test\"}").RootElement]);

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.Value);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Single(response.NonFatalErrors);
    }
}
