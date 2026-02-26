using System;
using System.Linq;
using System.Text.Json;
using Braze.Api.SubscriptionGroups;
using Braze.Api.UserData;
using Xunit;

namespace Braze.Api.Tests;

public class SubscriptionGroupsSerializationTests
{
    [Fact]
    public void SubscriptionStatusSetRequest_SerializesToJson()
    {
        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString("D"),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    ExternalIds = ["example-user", "[email protected]"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request, DefaultJsonSerializerOptions.Options);
        var expected = """{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","external_ids":["example-user","[email protected]"]}]}""";
        expected = expected.Replace("subscription_group_identifier", request.SubscriptionGroups.ElementAt(0).SubscriptionGroupId);

        Assert.Equal(expected, json);
    }

    [Fact]
    public void SubscriptionStatusSetRequest_WithEmails_SerializesToJson()
    {
        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString("D"),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    Emails = ["[email protected]", "[email protected]"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request, DefaultJsonSerializerOptions.Options);
        var expected = """{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","emails":["[email protected]","[email protected]"]}]}""";
        expected = expected.Replace("subscription_group_identifier", request.SubscriptionGroups.ElementAt(0).SubscriptionGroupId);
        Assert.Equal(expected, json);
    }

    [Fact]
    public void SubscriptionStatusSetRequest_WithPhones_SerializesToJson()
    {
        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString("D"),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    Phones = ["+12223334444", "+15556667777"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request, DefaultJsonSerializerOptions.Options);
        var expectedJson =
            """{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","phones":["+12223334444","+15556667777"]}]}""";
        expectedJson = expectedJson.Replace("subscription_group_identifier", request.SubscriptionGroups.ElementAt(0).SubscriptionGroupId);
        var expected = JsonDocument.Parse(expectedJson).RootElement;
        var actual = JsonDocument.Parse(json).RootElement;

        Assert.True(JsonElement.DeepEquals(expected, actual), $"'{actual}' not equal to '{expected}'");
    }

    [Fact]
    public void SubscriptionStatusSetRequest_WithMultipleGroups_SerializesToJson()
    {
        var group1 = Guid.NewGuid().ToString("D");
        var group2 = Guid.NewGuid().ToString("D");
        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = group1,
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    ExternalIds = ["user1"]
                },
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = group2,
                    SubscriptionState = SubscriptionGroupSubscribeState.Unsubscribed,
                    ExternalIds = ["user2"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request, DefaultJsonSerializerOptions.Options);
        var jsonDocument = JsonDocument.Parse(json);
        var subscriptionGroups = jsonDocument.RootElement.GetProperty("subscription_groups");

        Assert.Equal(2, subscriptionGroups.GetArrayLength());
    }

    [Fact]
    public void SubscriptionStatusSetRequest_WithDoubleOptIn_SerializesToJson()
    {
        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = Guid.NewGuid().ToString(),
                    SubscriptionState = SubscriptionGroupSubscribeState.Subscribed,
                    Phones = ["+12223334444"],
                    UseDoubleOptInLogic = true
                }
            ]
        };

        var json = JsonSerializer.Serialize(request, DefaultJsonSerializerOptions.Options);
        var expectedJson =
            """{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","phones":["+12223334444"],"use_double_opt_in_logic":true}]}""";
        expectedJson = expectedJson.Replace("subscription_group_identifier", request.SubscriptionGroups.ElementAt(0).SubscriptionGroupId);
        var expected = JsonDocument.Parse(expectedJson).RootElement;
        var actual = JsonDocument.Parse(json).RootElement;

        Assert.True(JsonElement.DeepEquals(expected, actual), $"'{actual}' not equal to '{expected}'");
    }

    [Fact]
    public void SubscriptionStatusSetResponse_DeserializesFromJson()
    {
        var json = """{"message":"success"}""";

        var response = JsonSerializer.Deserialize<SubscriptionStatusSetResponse>(
            json,
            DefaultJsonSerializerOptions.Options);

        Assert.NotNull(response);
        Assert.Equal("success", response.Message);
    }
}
