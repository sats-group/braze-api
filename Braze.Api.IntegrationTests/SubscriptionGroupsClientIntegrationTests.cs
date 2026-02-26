using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Braze.Api.SubscriptionGroups;
using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Integration tests for the SubscriptionGroupsClient.
/// </summary>
public class SubscriptionGroupsClientIntegrationTests
{
    [Fact]
    public async Task SetSubscriptionStatus_SendsCorrectHttpRequest()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateSubscriptionGroupsClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success""}");

        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    ExternalIds = ["user123"]
                }
            ]
        };

        // Act
        await client.SetSubscriptionStatus(request);

        // Assert
        Assert.NotNull(handler.LastRequest);
        handler.LastRequest.AssertMethod(HttpMethod.Post);
        handler.LastRequest.AssertUri("/v2/subscription/status/set");
        handler.LastRequest.AssertBearerToken("test-api-key");
        handler.LastRequest.AssertContentType("application/json");
    }

    [Fact]
    public async Task SetSubscriptionStatus_SerializesRequestBodyCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateSubscriptionGroupsClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success""}");

        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups = new List<SubscriptionGroupUpdate>
            {
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    ExternalIds = new List<string> { "user123", "user456" }
                }
            }
        };

        // Act
        await client.SetSubscriptionStatus(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "subscription_groups");
        Assert.True(root.TryGetProperty("subscription_groups", out var groups));
        Assert.Equal(JsonValueKind.Array, groups.ValueKind);

        var firstGroup = groups[0];
        HttpRequestAssertions.AssertJsonProperty(firstGroup, "subscription_group_id", request.SubscriptionGroups.ElementAt(0).SubscriptionGroupId);
        HttpRequestAssertions.AssertJsonProperty(firstGroup, "subscription_state", "subscribed");

        HttpRequestAssertions.AssertJsonPropertyExists(firstGroup, "external_ids");
        var externalIds = firstGroup.GetProperty("external_ids");
        Assert.Equal(JsonValueKind.Array, externalIds.ValueKind);
        Assert.Equal(2, externalIds.GetArrayLength());
        Assert.Equal("user123", externalIds[0].GetString());
        Assert.Equal("user456", externalIds[1].GetString());
    }

    [Fact]
    public async Task SetSubscriptionStatus_WithEmails_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateSubscriptionGroupsClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success""}");

        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups = new List<SubscriptionGroupUpdate>
            {
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Unsubscribed,
                    Emails = new List<string> { "test1@example.com", "test2@example.com" }
                }
            }
        };

        // Act
        await client.SetSubscriptionStatus(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var firstGroup = body.RootElement.GetProperty("subscription_groups")[0];

        HttpRequestAssertions.AssertJsonProperty(firstGroup, "subscription_state", "unsubscribed");
        HttpRequestAssertions.AssertJsonPropertyExists(firstGroup, "emails");

        var emails = firstGroup.GetProperty("emails");
        Assert.Equal(2, emails.GetArrayLength());
        Assert.Equal("test1@example.com", emails[0].GetString());
        Assert.Equal("test2@example.com", emails[1].GetString());
    }

    [Fact]
    public async Task SetSubscriptionStatus_WithPhones_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateSubscriptionGroupsClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success""}");

        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups = new List<SubscriptionGroupUpdate>
            {
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    Phones = new List<string> { "+1234567890" }
                }
            }
        };

        // Act
        await client.SetSubscriptionStatus(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var firstGroup = body.RootElement.GetProperty("subscription_groups")[0];

        HttpRequestAssertions.AssertJsonPropertyExists(firstGroup, "phones");
        var phones = firstGroup.GetProperty("phones");
        Assert.Single(phones.EnumerateArray());
        Assert.Equal("+1234567890", phones[0].GetString());
    }

    [Fact]
    public async Task SetSubscriptionStatus_SuccessResponse_ReturnsSuccessApiResponse()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateSubscriptionGroupsClient();
        handler.ConfigureSuccessResponse(
            @"{""message"": ""success""}",
            rateLimit: 10000,
            rateLimitRemaining: 9999,
            rateLimitReset: 60);

        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    ExternalIds = ["user123"]
                }
            ]
        };

        // Act
        var response = await client.SetSubscriptionStatus(request);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Equal(10000, response.RateLimitingLimit);
        Assert.Equal(9999, response.RateLimitingRemaining);
        Assert.Equal(60, response.RateLimitingReset);
    }

    [Fact]
    public async Task SetSubscriptionStatus_MultipleSubscriptionGroups_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateSubscriptionGroupsClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success""}");

        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups = new List<SubscriptionGroupUpdate>
            {
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    ExternalIds = new List<string> { "user123" }
                },
                new()
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Unsubscribed,
                    ExternalIds = new List<string> { "user456" }
                }
            }
        };

        // Act
        await client.SetSubscriptionStatus(request);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var groups = body.RootElement.GetProperty("subscription_groups");

        Assert.Equal(2, groups.GetArrayLength());
        HttpRequestAssertions.AssertJsonProperty(groups[0], "subscription_group_id", request.SubscriptionGroups.ElementAt(0).SubscriptionGroupId);
        HttpRequestAssertions.AssertJsonProperty(groups[1], "subscription_group_id", request.SubscriptionGroups.ElementAt(1).SubscriptionGroupId);
    }
}
