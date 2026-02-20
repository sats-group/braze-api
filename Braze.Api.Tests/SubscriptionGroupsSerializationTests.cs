using System.Collections.Generic;
using System.Text.Json;
using Braze.Api.SubscriptionGroups;
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
                    SubscriptionGroupId = "subscription_group_identifier",
                    SubscriptionState = "subscribed",
                    ExternalIds = ["example-user", "[email protected]"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request);
        var expected = """{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","external_ids":["example-user","[email protected]"]}]}""";

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
                    SubscriptionGroupId = "subscription_group_identifier",
                    SubscriptionState = "subscribed",
                    Emails = ["[email protected]", "[email protected]"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request);
        var expected = """{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","emails":["[email protected]","[email protected]"]}]}""";

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
                    SubscriptionGroupId = "subscription_group_identifier",
                    SubscriptionState = "subscribed",
                    Phones = ["+12223334444", "+15556667777"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request);
        var expected = JsonDocument.Parse("""{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","phones":["+12223334444","+15556667777"]}]}""").RootElement;
        var actual = JsonDocument.Parse(json).RootElement;

        Assert.True(JsonElement.DeepEquals(expected, actual), $"'{actual}' not equal to '{expected}'");
    }

    [Fact]
    public void SubscriptionStatusSetRequest_WithMultipleGroups_SerializesToJson()
    {
        var request = new SubscriptionStatusSetRequest
        {
            SubscriptionGroups =
            [
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = "group_1",
                    SubscriptionState = "subscribed",
                    ExternalIds = ["user1"]
                },
                new SubscriptionGroupUpdate
                {
                    SubscriptionGroupId = "group_2",
                    SubscriptionState = "unsubscribed",
                    ExternalIds = ["user2"]
                }
            ]
        };

        var json = JsonSerializer.Serialize(request);
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
                    SubscriptionGroupId = "subscription_group_identifier",
                    SubscriptionState = "subscribed",
                    Phones = ["+12223334444"],
                    UseDoubleOptInLogic = true
                }
            ]
        };

        var json = JsonSerializer.Serialize(request);
        var expected = JsonDocument.Parse("""{"subscription_groups":[{"subscription_group_id":"subscription_group_identifier","subscription_state":"subscribed","phones":["+12223334444"],"use_double_opt_in_logic":true}]}""").RootElement;
        var actual = JsonDocument.Parse(json).RootElement;

        Assert.True(JsonElement.DeepEquals(expected, actual), $"'{actual}' not equal to '{expected}'");
    }

    [Fact]
    public void SubscriptionStatusSetResponse_DeserializesFromJson()
    {
        var json = """{"message":"success"}""";

        var response = JsonSerializer.Deserialize<SubscriptionStatusSetResponse>(json);

        Assert.NotNull(response);
        Assert.Equal("success", response.Message);
    }
}
