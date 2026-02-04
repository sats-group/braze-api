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
        var response = new ApiResponse<string>("yolo", ["yolo"])
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
        var response = new ApiResponse<string>(null, ["yolo"])
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
}
