using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Braze.Api.Tests;

public class PropertySerializationTests
{
    private const string TestDateTimeString = "2024-01-01T00:00:00+00:00";

    [Fact]
    public void DeserializeStringProperty()
    {
        var json = "\"hello world\"";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.String>(property);
        Assert.Equal("hello world", ((Property.String)property).Value);
    }

    [Fact]
    public void DeserializeNullProperty()
    {
        var json = "null";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.Null(property);
    }

    [Fact]
    public void DeserializeIntegerProperty()
    {
        var json = "42";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Integer>(property);
        Assert.Equal(42, ((Property.Integer)property).Value);
    }

    [Fact]
    public void DeserializeFloatProperty()
    {
        var json = "42.5";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Float>(property);
        Assert.Equal(42.5, ((Property.Float)property).Value);
    }

    [Fact]
    public void DeserializeBoolTrueProperty()
    {
        var json = "true";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Bool>(property);
        Assert.True(((Property.Bool)property).Value);
    }

    [Fact]
    public void DeserializeBoolFalseProperty()
    {
        var json = "false";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Bool>(property);
        Assert.False(((Property.Bool)property).Value);
    }

    [Fact]
    public void DeserializeDateTimeOffsetProperty()
    {
        var json = $"\"{TestDateTimeString}\"";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Time>(property);
        var expected = DateTimeOffset.Parse(TestDateTimeString);
        Assert.Equal(expected, ((Property.Time)property).Value);
    }

    [Fact]
    public void DeserializeArrayProperty()
    {
        var json = "[1, \"hello\", true, 42.5]";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Array>(property);
        var array = ((Property.Array)property).Value;
        Assert.Equal(4, array.Count);
        Assert.IsType<Property.Integer>(array[0]);
        Assert.IsType<Property.String>(array[1]);
        Assert.IsType<Property.Bool>(array[2]);
        Assert.IsType<Property.Float>(array[3]);
    }

    [Fact]
    public void DeserializeStringThatLooksLikeYear()
    {
        // Strings like "2024" should remain as strings, not be parsed as dates
        var json = "\"2024\"";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.String>(property);
        Assert.Equal("2024", ((Property.String)property).Value);
    }

    [Fact]
    public void DeserializeStringThatLooksLikePartialDate()
    {
        // Strings like "March 2024" should remain as strings
        var json = "\"March 2024\"";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.String>(property);
        Assert.Equal("March 2024", ((Property.String)property).Value);
    }

    [Fact]
    public void DeserializeObjectProperty()
    {
        var json = "{\"name\": \"John\", \"age\": 30, \"active\": true}";
        var property = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(property);
        Assert.IsType<Property.Object>(property);
        var obj = ((Property.Object)property).Value;
        Assert.Equal(3, obj.Count);
        Assert.IsType<Property.String>(obj["name"]);
        Assert.Equal("John", ((Property.String)obj["name"]).Value);
        Assert.IsType<Property.Integer>(obj["age"]);
        Assert.Equal(30, ((Property.Integer)obj["age"]).Value);
        Assert.IsType<Property.Bool>(obj["active"]);
        Assert.True(((Property.Bool)obj["active"]).Value);
    }

    [Fact]
    public void SerializeAndDeserializeRoundTrip()
    {
        var original = new Property.Object
        {
            Value = new Dictionary<string, Property>
            {
                { "string", Property.Create("test") },
                { "integer", Property.Create(42) },
                { "float", Property.Create(42.5) },
                { "bool", Property.Create(true) },
                { "time", Property.Create(DateTimeOffset.Parse(TestDateTimeString)) },
                { "array", Property.Create(new List<Property> { Property.Create(1), Property.Create("nested") }) }
            }
        };

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<Property>(json);

        Assert.NotNull(deserialized);
        Assert.IsType<Property.Object>(deserialized);
        var obj = ((Property.Object)deserialized).Value;

        Assert.Equal(6, obj.Count);
        Assert.Equal("test", ((Property.String)obj["string"]).Value);
        Assert.Equal(42, ((Property.Integer)obj["integer"]).Value);
        Assert.Equal(42.5, ((Property.Float)obj["float"]).Value);
        Assert.True(((Property.Bool)obj["bool"]).Value);
        Assert.Equal(DateTimeOffset.Parse(TestDateTimeString), ((Property.Time)obj["time"]).Value);

        var array = ((Property.Array)obj["array"]).Value;
        Assert.Equal(2, array.Count);
        Assert.Equal(1, ((Property.Integer)array[0]).Value);
        Assert.Equal("nested", ((Property.String)array[1]).Value);
    }
}
