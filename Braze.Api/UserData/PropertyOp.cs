using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// The property request model.
/// </summary>
[JsonConverter(typeof(PropertyOpConverter))]
public abstract class PropertyOp
{
    private PropertyOp()
    {
    }

    /// <summary>
    /// Create a LiteralOp with the provided string value.
    /// </summary>
    public static PropertyOp Literal(string? value) =>
        new LiteralOp { Value = Property.Create(value), };

    /// <summary>
    /// Create a LiteralOp with the provided int value.
    /// </summary>
    public static PropertyOp Literal(int? value) =>
        new LiteralOp { Value = Property.Create(value), };

    /// <summary>
    /// Create a LiteralOp with the provided double value.
    /// </summary>
    public static PropertyOp Literal(double? value) =>
        new LiteralOp { Value = Property.Create(value), };

    /// <summary>
    /// Create a LiteralOp with the provided bool value.
    /// </summary>
    public static PropertyOp Literal(bool? value) =>
        new LiteralOp { Value = Property.Create(value), };

    /// <summary>
    /// Create a LiteralOp with the provided int value.
    /// </summary>
    public static PropertyOp Literal(DateTimeOffset? value) =>
        new LiteralOp { Value = Property.Create(value), };

    /// <summary>
    /// Implicit cast from nullable int to <see cref="Property"/>.
    /// </summary>
    public static implicit operator PropertyOp(int? value) => Literal(value);

    /// <summary>
    /// Implicit cast from nullable double to <see cref="Property"/>.
    /// </summary>
    public static implicit operator PropertyOp(double? value) => Literal(value);

    /// <summary>
    /// Implicit cast from nullable bool to <see cref="Property"/>.
    /// </summary>
    public static implicit operator PropertyOp(bool? value) => Literal(value);

    /// <summary>
    /// Implicit cast from nullable string to <see cref="Property"/>.
    /// </summary>
    public static implicit operator PropertyOp(string? value) => Literal(value);

    /// <summary>
    /// Implicit cast from nullable DateTimeOffset to <see cref="Property"/>.
    /// </summary>
    public static implicit operator PropertyOp(DateTimeOffset? value) => Literal(value);


    /// <summary>
    /// A literal value.
    /// </summary>
    [JsonConverter(typeof(PropertyOpConverter))]
    public class LiteralOp : PropertyOp
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required Property Value { get; init; }
    }

    /// <summary>
    /// An integer that should be incremented.
    /// </summary>
    [JsonConverter(typeof(PropertyOpConverter))]
    public class IncrementInteger : PropertyOp
    {
        /// <summary>
        /// The value to increment by.
        /// </summary>
        public required int IncrementValue { get; init; }
    }

    /// <summary>
    /// An array that should be modified.
    /// </summary>
    [JsonConverter(typeof(PropertyOpConverter))]
    public class ModifyArray : PropertyOp
    {
        /// <summary>
        /// The modification type.
        /// </summary>
        public required ModificationType Type { get; init; }
        /// <summary>
        /// The values to add or remove.
        /// </summary>
        public required List<Property> Values { get; init; }
    }
}

/// <summary>
/// The modification type.
/// </summary>
public enum ModificationType
{
    /// <summary>
    /// Add to the array.
    /// </summary>
    Add = 1,
    /// <summary>
    /// Remove from the array.
    /// </summary>
    Remove = 2,
}

internal class PropertyOpConverter : JsonConverter<PropertyOp>
{
    public override PropertyOp? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        throw new NotImplementedException();

    public override void Write(
        Utf8JsonWriter writer,
        PropertyOp value,
        JsonSerializerOptions options)
    {
        switch (value)
        {
            case PropertyOp.LiteralOp literal:
                JsonSerializer.Serialize(writer, literal.Value, options);
                break;
            case PropertyOp.IncrementInteger inc:
                writer.WriteStartObject();
                writer.WritePropertyName("inc");
                writer.WriteNumberValue(inc.IncrementValue);
                writer.WriteEndObject();
                break;
            case PropertyOp.ModifyArray modify:
                writer.WriteStartObject();
                switch (modify.Type)
                {
                    case ModificationType.Add:
                        writer.WritePropertyName("add");
                        break;
                    case ModificationType.Remove:
                        writer.WritePropertyName("remove");
                        break;
                    default:
                        throw new InvalidOperationException($"{nameof(modify.Type)}: not supported {nameof(ModificationType)}.");
                }
                writer.WriteStartArray();
                foreach (var item in modify.Values)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(PropertyOp));
}

