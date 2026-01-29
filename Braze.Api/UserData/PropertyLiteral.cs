using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// A literal value that can be a string, integer, float, time, array or object.
/// </summary>
[JsonConverter(typeof(PropertyLiteralConverter))]
public abstract class PropertyLiteral
{
    private PropertyLiteral()
    {
    }

    /// <summary>
    /// Creates a string literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static PropertyLiteral Create(string value) => new String() { Value = value };

    /// <summary>
    /// Creates an integer literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static PropertyLiteral Create(int value) => new Integer() { Value = value };

    /// <summary>
    /// Creates a float literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static PropertyLiteral Create(double value) => new Float() { Value = value };

    /// <summary>
    /// Creates a time literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static PropertyLiteral Create(DateTimeOffset value) => new Time() { Value = value };

    /// <summary>
    /// Creates an array literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static PropertyLiteral Create(List<PropertyLiteral> value) => new Array() { Value = value };

    /// <summary>
    /// Creates an object literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static PropertyLiteral Create(Dictionary<string, PropertyLiteral> value) => new Object() { Value = value };

    /// <summary>
    /// An integer literal.
    /// </summary>
    [JsonConverter(typeof(PropertyLiteralConverter))]
    public class Integer : PropertyLiteral
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required int Value { get; init; }
    }

    /// <summary>
    /// A float literal.
    /// </summary>
    [JsonConverter(typeof(PropertyLiteralConverter))]
    public class Float : PropertyLiteral
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required double Value { get; init; }
    }

    /// <summary>
    /// A time literal.
    /// </summary>
    [JsonConverter(typeof(PropertyLiteralConverter))]
    public class Time : PropertyLiteral
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required DateTimeOffset Value { get; init; }
    }

    /// <summary>
    /// A string literal.
    /// </summary>
    [JsonConverter(typeof(PropertyLiteralConverter))]
    public class String : PropertyLiteral
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required string Value { get; init; }
    }

    /// <summary>
    /// An array literal.
    /// </summary>
    [JsonConverter(typeof(PropertyLiteralConverter))]
    public class Array : PropertyLiteral
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required List<PropertyLiteral> Value { get; init; }
    }

    /// <summary>
    /// An object literal.
    /// </summary>
    [JsonConverter(typeof(PropertyLiteralConverter))]
    public class Object : PropertyLiteral
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required Dictionary<string, PropertyLiteral> Value { get; init; }
    }
}

internal class PropertyLiteralConverter : JsonConverter<PropertyLiteral>
{
    public override PropertyLiteral? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        throw new NotImplementedException();

    public override void Write(
        Utf8JsonWriter writer,
        PropertyLiteral value,
        JsonSerializerOptions options)
    {
        switch (value)
        {
            case PropertyLiteral.Float number:
                writer.WriteNumberValue(number.Value);
                break;
            case PropertyLiteral.Integer integer:
                writer.WriteNumberValue(integer.Value);
                break;
            case PropertyLiteral.Time time:
                writer.WriteStringValue(time.Value);
                break;
            case PropertyLiteral.String str:
                writer.WriteStringValue(str.Value);
                break;
            case PropertyLiteral.Array arr:
                writer.WriteStartArray();
                foreach (var item in arr.Value)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
                return;

            case PropertyLiteral.Object obj:
                writer.WriteStartObject();
                foreach (var (key, child) in obj.Value)
                {
                    writer.WritePropertyName(key);
                    JsonSerializer.Serialize(writer, child, options);
                }
                writer.WriteEndObject();
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(PropertyLiteral));
}

