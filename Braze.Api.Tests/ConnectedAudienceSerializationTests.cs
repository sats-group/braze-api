using System.Collections.Generic;
using System.Text.Json;
using Braze.Api.Messages.Send;
using Xunit;

namespace Braze.Api.Tests;

public class ConnectedAudienceSerializationTests
{
    [Fact]
    public void Serialize_SegmentFilter_ProducesExpectedJson()
    {
        var audience = new ConnectedAudience.SegmentFilter
        {
            SegmentId = "segment-abc"
        };

        var json = JsonSerializer.Serialize<ConnectedAudience>(audience);

        Assert.Equal(@"{""segment"":{""segment_id"":""segment-abc""}}", json);
    }

    [Fact]
    public void Serialize_AndCompound_ProducesExpectedJson()
    {
        var audience = ConnectedAudience.CreateAnd(new List<ConnectedAudience>
        {
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-1" },
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-2" },
        });

        var json = JsonSerializer.Serialize(audience);

        Assert.Equal(@"{""AND"":[{""segment"":{""segment_id"":""seg-1""}},{""segment"":{""segment_id"":""seg-2""}}]}", json);
    }

    [Fact]
    public void Serialize_OrCompound_ProducesExpectedJson()
    {
        var audience = ConnectedAudience.CreateOr(new List<ConnectedAudience>
        {
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-1" },
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-2" },
        });

        var json = JsonSerializer.Serialize(audience);

        Assert.Equal(@"{""OR"":[{""segment"":{""segment_id"":""seg-1""}},{""segment"":{""segment_id"":""seg-2""}}]}", json);
    }

    [Fact]
    public void Serialize_NestedAndOr_ProducesExpectedJson()
    {
        // Represents the example from https://www.braze.com/docs/api/objects_filters/connected_audience/#how-it-works
        var audience = ConnectedAudience.CreateAnd(new List<ConnectedAudience>
        {
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-1" },
            ConnectedAudience.CreateOr(new List<ConnectedAudience>
            {
                new ConnectedAudience.SegmentFilter { SegmentId = "seg-2" },
                new ConnectedAudience.SegmentFilter { SegmentId = "seg-3" },
            }),
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-4" },
        });

        var json = JsonSerializer.Serialize(audience);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("AND", out var andProp));
        Assert.Equal(JsonValueKind.Array, andProp.ValueKind);
        Assert.Equal(3, andProp.GetArrayLength());

        // First element: segment seg-1
        Assert.True(andProp[0].TryGetProperty("segment", out var firstSegment));
        Assert.Equal("seg-1", firstSegment.GetProperty("segment_id").GetString());

        // Second element: OR of seg-2 and seg-3
        Assert.True(andProp[1].TryGetProperty("OR", out var orProp));
        Assert.Equal(2, orProp.GetArrayLength());
        Assert.True(orProp[0].TryGetProperty("segment", out var orSeg1));
        Assert.Equal("seg-2", orSeg1.GetProperty("segment_id").GetString());
        Assert.True(orProp[1].TryGetProperty("segment", out var orSeg2));
        Assert.Equal("seg-3", orSeg2.GetProperty("segment_id").GetString());

        // Third element: segment seg-4
        Assert.True(andProp[2].TryGetProperty("segment", out var thirdSegment));
        Assert.Equal("seg-4", thirdSegment.GetProperty("segment_id").GetString());
    }

    [Fact]
    public void Serialize_CustomAttributeFilter_WithValue_ProducesExpectedJson()
    {
        var audience = new ConnectedAudience.CustomAttributeFilter
        {
            CustomAttributeDefinitionId = "my_attribute",
            Comparison = "equals",
            Value = Property.Create("test_value"),
        };

        var json = JsonSerializer.Serialize<ConnectedAudience>(audience);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("custom_attribute", out var customAttr));
        Assert.Equal("my_attribute", customAttr.GetProperty("custom_attribute_definition_id").GetString());
        Assert.Equal("equals", customAttr.GetProperty("comparison").GetString());
        Assert.Equal("test_value", customAttr.GetProperty("value").GetString());
    }

    [Fact]
    public void Serialize_CustomAttributeFilter_WithoutValue_OmitsValueProperty()
    {
        var audience = new ConnectedAudience.CustomAttributeFilter
        {
            CustomAttributeDefinitionId = "my_attribute",
            Comparison = "exists",
        };

        var json = JsonSerializer.Serialize<ConnectedAudience>(audience);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("custom_attribute", out var customAttr));
        Assert.False(customAttr.TryGetProperty("value", out _));
    }

    [Fact]
    public void Serialize_PushSubscriptionStatusFilter_ProducesExpectedJson()
    {
        var audience = new ConnectedAudience.PushSubscriptionStatusFilter
        {
            Comparison = "is",
            Value = "opted_in",
        };

        var json = JsonSerializer.Serialize<ConnectedAudience>(audience);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("push_subscription_status", out var push));
        Assert.Equal("is", push.GetProperty("comparison").GetString());
        Assert.Equal("opted_in", push.GetProperty("value").GetString());
    }

    [Fact]
    public void Serialize_EmailSubscriptionStatusFilter_ProducesExpectedJson()
    {
        var audience = new ConnectedAudience.EmailSubscriptionStatusFilter
        {
            Comparison = "is",
            Value = "subscribed",
        };

        var json = JsonSerializer.Serialize<ConnectedAudience>(audience);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("email_subscription_status", out var email));
        Assert.Equal("is", email.GetProperty("comparison").GetString());
        Assert.Equal("subscribed", email.GetProperty("value").GetString());
    }

    [Fact]
    public void Deserialize_SegmentFilter_ReturnsCorrectType()
    {
        var json = @"{""segment"":{""segment_id"":""segment-abc""}}";

        var audience = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var segmentFilter = Assert.IsType<ConnectedAudience.SegmentFilter>(audience);
        Assert.Equal("segment-abc", segmentFilter.SegmentId);
    }

    [Fact]
    public void Deserialize_AndCompound_ReturnsCorrectType()
    {
        var json = @"{""AND"":[{""segment"":{""segment_id"":""seg-1""}},{""segment"":{""segment_id"":""seg-2""}}]}";

        var audience = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var and = Assert.IsType<ConnectedAudience.And>(audience);
        var filters = new List<ConnectedAudience>(and.Filters);
        Assert.Equal(2, filters.Count);
        Assert.IsType<ConnectedAudience.SegmentFilter>(filters[0]);
        Assert.IsType<ConnectedAudience.SegmentFilter>(filters[1]);
    }

    [Fact]
    public void Deserialize_OrCompound_ReturnsCorrectType()
    {
        var json = @"{""OR"":[{""segment"":{""segment_id"":""seg-1""}},{""segment"":{""segment_id"":""seg-2""}}]}";

        var audience = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var or = Assert.IsType<ConnectedAudience.Or>(audience);
        var filters = new List<ConnectedAudience>(or.Filters);
        Assert.Equal(2, filters.Count);
    }

    [Fact]
    public void Deserialize_CustomAttributeFilter_ReturnsCorrectType()
    {
        var json = @"{""custom_attribute"":{""custom_attribute_definition_id"":""my_attr"",""comparison"":""equals"",""value"":""hello""}}";

        var audience = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var customAttr = Assert.IsType<ConnectedAudience.CustomAttributeFilter>(audience);
        Assert.Equal("my_attr", customAttr.CustomAttributeDefinitionId);
        Assert.Equal("equals", customAttr.Comparison);
        Assert.IsType<Property.String>(customAttr.Value);
    }

    [Fact]
    public void Deserialize_PushSubscriptionStatusFilter_ReturnsCorrectType()
    {
        var json = @"{""push_subscription_status"":{""comparison"":""is"",""value"":""opted_in""}}";

        var audience = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var push = Assert.IsType<ConnectedAudience.PushSubscriptionStatusFilter>(audience);
        Assert.Equal("is", push.Comparison);
        Assert.Equal("opted_in", push.Value);
    }

    [Fact]
    public void Deserialize_EmailSubscriptionStatusFilter_ReturnsCorrectType()
    {
        var json = @"{""email_subscription_status"":{""comparison"":""is"",""value"":""subscribed""}}";

        var audience = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var email = Assert.IsType<ConnectedAudience.EmailSubscriptionStatusFilter>(audience);
        Assert.Equal("is", email.Comparison);
        Assert.Equal("subscribed", email.Value);
    }

    [Fact]
    public void RoundTrip_NestedAndOr_PreservesStructure()
    {
        var original = ConnectedAudience.CreateAnd(new List<ConnectedAudience>
        {
            new ConnectedAudience.SegmentFilter { SegmentId = "seg-1" },
            ConnectedAudience.CreateOr(new List<ConnectedAudience>
            {
                new ConnectedAudience.SegmentFilter { SegmentId = "seg-2" },
                new ConnectedAudience.CustomAttributeFilter
                {
                    CustomAttributeDefinitionId = "my_attr",
                    Comparison = "equals",
                    Value = Property.Create("hello"),
                },
            }),
        });

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<ConnectedAudience>(json);

        var and = Assert.IsType<ConnectedAudience.And>(deserialized);
        var andFilters = new List<ConnectedAudience>(and.Filters);
        Assert.Equal(2, andFilters.Count);

        Assert.IsType<ConnectedAudience.SegmentFilter>(andFilters[0]);
        var or = Assert.IsType<ConnectedAudience.Or>(andFilters[1]);

        var orFilters = new List<ConnectedAudience>(or.Filters);
        Assert.Equal(2, orFilters.Count);
        Assert.IsType<ConnectedAudience.SegmentFilter>(orFilters[0]);
        var customAttr = Assert.IsType<ConnectedAudience.CustomAttributeFilter>(orFilters[1]);
        Assert.Equal("my_attr", customAttr.CustomAttributeDefinitionId);
    }
}
