using System.Text.Json;
using Xunit;

namespace Braze.Api.Tests;

public class ApiResponseTests
{
    [Fact]
    public void SuccessTestWhenErrorsIsNull()
    {
        var response = new ApiResponse<string>("yolo", null)
        {
            RateLimitingLimit = 0,
            RateLimitingRemaining = 0,
            RateLimitingReset = 0
        };

        Assert.True(response.Success);
    }

    [Fact]
    public void SuccessTestWhenErrorsIsEmpty()
    {
        var response = new ApiResponse<string>("yolo", [])
        {
            RateLimitingLimit = 0,
            RateLimitingRemaining = 0,
            RateLimitingReset = 0
        };

        Assert.True(response.Success);
    }

    [Fact]
    public void SuccessTestWhenErrorsIsNonEmpty()
    {
        var response = new ApiResponse<string>("yolo", [JsonDocument.Parse("{}").RootElement])
        {
            RateLimitingLimit = 0,
            RateLimitingRemaining = 0,
            RateLimitingReset = 0
        };

        Assert.False(response.Success);
    }

    [Fact]
    public void SuccessTestWhenValueIsNull()
    {
        var response = new ApiResponse<string>(null, [JsonDocument.Parse("{}").RootElement])
        {
            RateLimitingLimit = 0,
            RateLimitingRemaining = 0,
            RateLimitingReset = 0
        };

        Assert.False(response.Success);
    }

    [Fact]
    public void SuccessTestWhenValueAndNonFatalErrorsIsNull()
    {
        var response = new ApiResponse<string>(null, null)
        {
            RateLimitingLimit = 0,
            RateLimitingRemaining = 0,
            RateLimitingReset = 0
        };

        Assert.False(response.Success);
    }

    [Fact]
    public void CreateSuccess_ReturnsSuccessfulResponse()
    {
        var response = ApiResponse<string>.CreateSuccess("test-value");

        Assert.True(response.Success);
        Assert.Equal("test-value", response.Value);
        Assert.Null(response.NonFatalErrors);
        Assert.Equal(0, response.RateLimitingLimit);
        Assert.Equal(0, response.RateLimitingRemaining);
        Assert.Equal(0, response.RateLimitingReset);
    }

    [Fact]
    public void CreateSuccess_WithRateLimitingValues_SetsRateLimitingProperties()
    {
        var response = ApiResponse<string>.CreateSuccess(
            "test-value",
            rateLimitingLimit: 100,
            rateLimitingRemaining: 50,
            rateLimitingReset: 30);

        Assert.True(response.Success);
        Assert.Equal("test-value", response.Value);
        Assert.Equal(100, response.RateLimitingLimit);
        Assert.Equal(50, response.RateLimitingRemaining);
        Assert.Equal(30, response.RateLimitingReset);
    }

    [Fact]
    public void CreateWithErrors_WithErrorsOnly_ReturnsFailedResponse()
    {
        var errors = new System.Collections.Generic.List<JsonElement>
        {
            JsonDocument.Parse(@"{""error"": ""test error""}").RootElement
        };

        var response = ApiResponse<string>.CreateWithErrors(errors);

        Assert.False(response.Success);
        Assert.Null(response.Value);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Single(response.NonFatalErrors);
        Assert.Equal(0, response.RateLimitingLimit);
        Assert.Equal(0, response.RateLimitingRemaining);
        Assert.Equal(0, response.RateLimitingReset);
    }

    [Fact]
    public void CreateWithErrors_WithValueAndErrors_ReturnsFailedResponse()
    {
        var errors = new System.Collections.Generic.List<JsonElement>
        {
            JsonDocument.Parse(@"{""error"": ""test error""}").RootElement
        };

        var response = ApiResponse<string>.CreateWithErrors(errors, "partial-value");

        Assert.False(response.Success);
        Assert.Equal("partial-value", response.Value);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Single(response.NonFatalErrors);
    }

    [Fact]
    public void CreateWithErrors_WithRateLimitingValues_SetsRateLimitingProperties()
    {
        var errors = new System.Collections.Generic.List<JsonElement>
        {
            JsonDocument.Parse(@"{""error"": ""test error""}").RootElement
        };

        var response = ApiResponse<string>.CreateWithErrors(
            errors,
            rateLimitingLimit: 100,
            rateLimitingRemaining: 50,
            rateLimitingReset: 30);

        Assert.False(response.Success);
        Assert.Equal(100, response.RateLimitingLimit);
        Assert.Equal(50, response.RateLimitingRemaining);
        Assert.Equal(30, response.RateLimitingReset);
    }

    [Fact]
    public void CreateWithErrors_WithEmptyErrorsList_ReturnsSuccessfulResponse()
    {
        var errors = new System.Collections.Generic.List<JsonElement>();

        var response = ApiResponse<string>.CreateWithErrors(errors, "value");

        Assert.True(response.Success);
        Assert.Equal("value", response.Value);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Empty(response.NonFatalErrors);
    }
}
