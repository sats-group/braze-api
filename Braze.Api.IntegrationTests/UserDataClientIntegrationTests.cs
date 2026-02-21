using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Integration tests for the UserDataClient.
/// </summary>
public class UserDataClientIntegrationTests
{
    [Fact]
    public async Task Track_SendsCorrectHttpRequest()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""attributes_processed"": 1}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute>
            {
                new UserAttribute
                {
                    ExternalId = "user123",
                    Email = "test@example.com",
                    FirstName = "John",
                    LastName = "Doe"
                }
            }
        };

        // Act
        var response = await client.Track(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        handler.LastRequest.AssertMethod(HttpMethod.Post);
        handler.LastRequest.AssertUri("users/track");
        handler.LastRequest.AssertBearerToken("test-api-key");
        handler.LastRequest.AssertContentType("application/json");
    }

    [Fact]
    public async Task Track_SerializesRequestBodyCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""attributes_processed"": 1}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute>
            {
                new UserAttribute
                {
                    ExternalId = "user123",
                    Email = "test@example.com",
                    Country = "US",
                    Gender = Gender.Male
                }
            }
        };

        // Act
        await client.Track(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "attributes");
        Assert.True(root.TryGetProperty("attributes", out var attributes));
        Assert.Equal(JsonValueKind.Array, attributes.ValueKind);

        var firstAttribute = attributes[0];
        HttpRequestAssertions.AssertJsonProperty(firstAttribute, "external_id", "user123");
        HttpRequestAssertions.AssertJsonProperty(firstAttribute, "email", "test@example.com");
        HttpRequestAssertions.AssertJsonProperty(firstAttribute, "country", "US");
        HttpRequestAssertions.AssertJsonProperty(firstAttribute, "gender", "M");
    }

    [Fact]
    public async Task Track_WithEvents_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""events_processed"": 1}");

        var request = new TrackRequest
        {
            Events = new List<Event>
            {
                new Event
                {
                    ExternalId = "user123",
                    Name = "test_event",
                    Time = DateTimeOffset.Parse("2024-01-01T00:00:00Z")
                }
            }
        };

        // Act
        await client.Track(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "events");
        Assert.True(root.TryGetProperty("events", out var events));
        Assert.Equal(JsonValueKind.Array, events.ValueKind);

        var firstEvent = events[0];
        HttpRequestAssertions.AssertJsonProperty(firstEvent, "external_id", "user123");
        HttpRequestAssertions.AssertJsonProperty(firstEvent, "name", "test_event");
        HttpRequestAssertions.AssertJsonPropertyExists(firstEvent, "time");
    }

    [Fact]
    public async Task Track_WithPurchases_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""purchases_processed"": 1}");

        var request = new TrackRequest
        {
            Purchases = new List<Purchase>
            {
                new Purchase
                {
                    ExternalId = "user123",
                    ProductId = "product123",
                    Currency = "USD",
                    Price = 9.99m,
                    Time = DateTimeOffset.Parse("2024-01-01T00:00:00Z")
                }
            }
        };

        // Act
        await client.Track(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "purchases");
        Assert.True(root.TryGetProperty("purchases", out var purchases));
        Assert.Equal(JsonValueKind.Array, purchases.ValueKind);

        var firstPurchase = purchases[0];
        HttpRequestAssertions.AssertJsonProperty(firstPurchase, "external_id", "user123");
        HttpRequestAssertions.AssertJsonProperty(firstPurchase, "product_id", "product123");
        HttpRequestAssertions.AssertJsonProperty(firstPurchase, "currency", "USD");
        HttpRequestAssertions.AssertJsonPropertyExists(firstPurchase, "price");
        HttpRequestAssertions.AssertJsonPropertyExists(firstPurchase, "time");
    }

    [Fact]
    public async Task Track_SuccessResponse_ReturnsSuccessApiResponse()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(
            @"{""message"": ""success"", ""attributes_processed"": 1}",
            rateLimit: 10000,
            rateLimitRemaining: 9999,
            rateLimitReset: 60);

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute>
            {
                new UserAttribute { ExternalId = "user123" }
            }
        };

        // Act
        var response = await client.Track(request);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Equal(1, response.Value.AttributesProcessed);
        Assert.Equal(10000, response.RateLimitingLimit);
        Assert.Equal(9999, response.RateLimitingRemaining);
        Assert.Equal(60, response.RateLimitingReset);
    }

    [Fact]
    public async Task Track_SuccessWithNonFatalErrors_ReturnsNonSuccessApiResponse()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(
            @"{""message"": ""success"", ""errors"": [""minor error""], ""attributes_processed"": 0}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute>
            {
                new UserAttribute { ExternalId = "user123" }
            }
        };

        // Act
        var response = await client.Track(request);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.NonFatalErrors);
        Assert.Single(response.NonFatalErrors);
    }

    [Fact]
    public async Task Track_NullPropertiesNotSerialized()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateUserDataClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success""}");

        var request = new TrackRequest
        {
            Attributes = new List<UserAttribute>
            {
                new UserAttribute
                {
                    ExternalId = "user123"
                    // FirstName, LastName, etc. are null
                }
            }
        };

        // Act
        await client.Track(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;
        var firstAttribute = root.GetProperty("attributes")[0];

        HttpRequestAssertions.AssertJsonPropertyExists(firstAttribute, "external_id");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(firstAttribute, "first_name");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(firstAttribute, "last_name");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(firstAttribute, "email");
    }
}
