using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api;

/// <summary>
/// A literal value that can be a string, integer, float, time, array or object.
/// </summary>
[JsonConverter(typeof(PropertyConverter))]
public abstract class Property
{
    private Property()
    {
    }

    /// <summary>
    /// Creates a string literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(string? value) => new String() { Value = value };

    /// <summary>
    /// Creates an integer literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(int? value) => new Integer() { Value = value };

    /// <summary>
    /// Creates a bool literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(bool? value) => new Bool() { Value = value };

    /// <summary>
    /// Creates a float literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(double? value) => new Float() { Value = value };

    /// <summary>
    /// Creates a time literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(DateTimeOffset? value) => new Time() { Value = value };

    /// <summary>
    /// Creates an array literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(List<Property> value) => new Array() { Value = value };

    /// <summary>
    /// Creates an object literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The literal.</returns>
    public static Property Create(Dictionary<string, Property> value) => new Object() { Value = value };

    /// <summary>
    /// Implicit cast from nullable int to <see cref="Property"/>.
    /// </summary>
    public static implicit operator Property(int? value) => Create(value);

    /// <summary>
    /// Implicit cast from nullable double to <see cref="Property"/>.
    /// </summary>
    public static implicit operator Property(double? value) => Create(value);

    /// <summary>
    /// Implicit cast from nullable bool to <see cref="Property"/>.
    /// </summary>
    public static implicit operator Property(bool? value) => Create(value);

    /// <summary>
    /// Implicit cast from nullable string to <see cref="Property"/>.
    /// </summary>
    public static implicit operator Property(string? value) => Create(value);

    /// <summary>
    /// Implicit cast from nullable DateTimeOffset to <see cref="Property"/>.
    /// </summary>
    public static implicit operator Property(DateTimeOffset? value) => Create(value);

    /// <summary>
    /// An integer literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class Integer : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required int? Value { get; init; }
    }

    /// <summary>
    /// A bool literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class Bool : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required bool? Value { get; init; }
    }

    /// <summary>
    /// A float literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class Float : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required double? Value { get; init; }
    }

    /// <summary>
    /// A time literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class Time : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required DateTimeOffset? Value { get; init; }
    }

    /// <summary>
    /// A string literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class String : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required string? Value { get; init; }
    }

    /// <summary>
    /// An array literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class Array : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required List<Property> Value { get; init; }
    }

    /// <summary>
    /// An object literal.
    /// </summary>
    [JsonConverter(typeof(PropertyConverter))]
    public class Object : Property
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required Dictionary<string, Property> Value { get; init; }
    }
}

internal class PropertyConverter : JsonConverter<Property>
{
    private const int MinIso8601Length = 10;

    public override Property? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var stringValue = reader.GetString();
                // Try to parse as DateTimeOffset only if it's in ISO 8601 format
                // This ensures we only parse strings that were likely serialized from Property.Time
                if (stringValue != null &&
                    stringValue.Length > MinIso8601Length &&
                    IsIso8601Format(stringValue) &&
                    DateTimeOffset.TryParse(stringValue, out var dateTimeOffset))
                {
                    return new Property.Time() { Value = dateTimeOffset };
                }
                return new Property.String() { Value = stringValue };

            case JsonTokenType.Number:
                // Check if it's an integer or float
                if (reader.TryGetInt32(out var intValue))
                {
                    return new Property.Integer() { Value = intValue };
                }
                return new Property.Float() { Value = reader.GetDouble() };

            case JsonTokenType.True:
                return new Property.Bool() { Value = true };

            case JsonTokenType.False:
                return new Property.Bool() { Value = false };

            case JsonTokenType.Null:
                return null;

            case JsonTokenType.StartArray:
                var list = new List<Property>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    var item = JsonSerializer.Deserialize<Property>(ref reader, options);
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
                return new Property.Array() { Value = list };

            case JsonTokenType.StartObject:
                var dict = new Dictionary<string, Property>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = reader.GetString();
                        ArgumentNullException.ThrowIfNull(propertyName);
                        reader.Read();
                        var propertyValue = JsonSerializer.Deserialize<Property>(ref reader, options);
                        if (propertyValue != null)
                        {
                            dict[propertyName] = propertyValue;
                        }
                    }
                }
                return new Property.Object() { Value = dict };

            default:
                throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }
    }

    private static bool IsIso8601Format(string value) =>
        value.Contains('T') &&
        (value.Contains('+') || value.Contains('-') || value.EndsWith('Z'));

    public override void Write(
        Utf8JsonWriter writer,
        Property value,
        JsonSerializerOptions options)
    {
        switch (value)
        {
            case Property.Float number:
                if (number.Value.HasValue)
                {
                    writer.WriteNumberValue(number.Value.Value);
                }
                else
                {
                    writer.WriteNullValue();
                }
                break;
            case Property.Integer integer:
                if (integer.Value.HasValue)
                {
                    writer.WriteNumberValue(integer.Value.Value);
                }
                else
                {
                    writer.WriteNullValue();
                }
                break;
            case Property.Time time:
                if (time.Value.HasValue)
                {
                    writer.WriteStringValue(time.Value.Value);
                }
                else
                {
                    writer.WriteNullValue();
                }
                break;
            case Property.String str:
                writer.WriteStringValue(str.Value);
                break;
            case Property.Bool boolean:
                if (boolean.Value.HasValue)
                {
                    writer.WriteBooleanValue(boolean.Value.Value);
                }
                else
                {
                    writer.WriteNullValue();
                }
                break;
            case Property.Array arr:
                writer.WriteStartArray();
                foreach (var item in arr.Value)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
                return;

            case Property.Object obj:
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

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(Property));
}

