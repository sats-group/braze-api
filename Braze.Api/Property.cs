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
    public override Property? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        throw new NotImplementedException();

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

