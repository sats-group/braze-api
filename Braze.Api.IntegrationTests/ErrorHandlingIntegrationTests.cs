using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Integration tests for error handling across all Braze API clients.
/// Tests various error scenarios documented in the Braze API specification.
/// </summary>
public class ErrorHandlingIntegrationTests
{
    #region Authentication and Authorization Errors (401, 403)

    [Fact]
    public async Task UnauthorizedError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.Unauthorized,
            @"{""message"": ""Invalid API key""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Invalid API key", exception.Message);
        Assert.Equal(HttpStatusCode.Unauthorized, exception.HttpStatusCode);
    }

    [Fact]
    public async Task ForbiddenError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.Forbidden,
            @"{""message"": ""The rate plan doesn't support this operation""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("The rate plan doesn't support this operation", exception.Message);
        Assert.Equal(HttpStatusCode.Forbidden, exception.HttpStatusCode);
    }

    [Fact]
    public async Task AccessDeniedError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.Forbidden,
            @"{""message"": ""The REST API key does not have sufficient permissions""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Contains("permissions", exception.Message);
        Assert.Equal(HttpStatusCode.Forbidden, exception.HttpStatusCode);
    }

    #endregion

    #region Client Errors (4XX)

    [Fact]
    public async Task BadRequest_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.BadRequest,
            @"{""message"": ""Bad syntax""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Bad syntax", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.HttpStatusCode);
    }

    [Fact]
    public async Task BadRequest_WithErrors_ThrowsBrazeApiExceptionWithErrors()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.BadRequest,
            @"{""message"": ""No Recipients"", ""errors"": [""error detail 1"", ""error detail 2""]}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("No Recipients", exception.Message);
        Assert.NotNull(exception.Errors);
        Assert.Equal(2, exception.Errors.Count);
    }

    [Fact]
    public async Task NotFoundError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.NotFound,
            @"{""message"": ""Invalid URL""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Invalid URL", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.HttpStatusCode);
    }

    #endregion

    #region Rate Limiting (429)

    [Fact]
    public async Task RateLimited_ThrowsBrazeApiExceptionWithRetryAfter()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureRateLimitedResponse(
            @"{""message"": ""Rate Limited""}",
            retryAfter: 5);

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Rate Limited", exception.Message);
        Assert.Equal(HttpStatusCode.TooManyRequests, exception.HttpStatusCode);
        Assert.Equal(5, exception.RateLimitingRetryAfter);
    }

    [Fact]
    public async Task RateLimited_WithLargeRetryAfter_PreservesValue()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureRateLimitedResponse(
            @"{""message"": ""Rate Limited""}",
            retryAfter: 3600); // 1 hour

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal(3600, exception.RateLimitingRetryAfter);
    }

    #endregion

    #region Server Errors (5XX)

    [Fact]
    public async Task InternalServerError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.InternalServerError,
            @"{""message"": ""Internal Server Error""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Internal Server Error", exception.Message);
        Assert.Equal(HttpStatusCode.InternalServerError, exception.HttpStatusCode);
    }

    [Fact]
    public async Task BadGatewayError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.BadGateway,
            @"{""message"": ""Bad Gateway""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal(HttpStatusCode.BadGateway, exception.HttpStatusCode);
    }

    [Fact]
    public async Task ServiceUnavailableError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.ServiceUnavailable,
            @"{""message"": ""Service Unavailable""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Service Unavailable", exception.Message);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.HttpStatusCode);
    }

    [Fact]
    public async Task GatewayTimeoutError_ThrowsBrazeApiException()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.GatewayTimeout,
            @"{""message"": ""Gateway Timeout""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Equal("Gateway Timeout", exception.Message);
        Assert.Equal(HttpStatusCode.GatewayTimeout, exception.HttpStatusCode);
    }

    #endregion

    #region Error Response Without Message

    [Fact]
    public async Task ErrorResponse_WithoutMessage_ThrowsWithDefaultMessage()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureResponse(
            HttpStatusCode.BadRequest,
            @"{}"); // No message field

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute> { new UserAttribute { ExternalId = "user123" } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.Track(request));
        Assert.Contains("Unknown error response returned from Braze", exception.Message);
        Assert.Contains("users/track", exception.Message);
    }

    #endregion
}
