using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.UserData;

/// <summary>
/// The property request model.
/// </summary>
[JsonConverter(typeof(PropertyRequestModelConverter))]
public abstract class PropertyRequestModel
{
    private PropertyRequestModel()
    {
    }

    /// <summary>
    /// A literal value.
    /// </summary>
    [JsonConverter(typeof(PropertyRequestModelConverter))]
    public class Literal : PropertyRequestModel
    {
        /// <summary>
        /// The value.
        /// </summary>
        public required PropertyLiteral Value { get; init; }
    }

    /// <summary>
    /// An integer that should be incremented.
    /// </summary>
    [JsonConverter(typeof(PropertyRequestModelConverter))]
    public class IncrementInteger : PropertyRequestModel
    {
        /// <summary>
        /// The value to increment by.
        /// </summary>
        public required int IncrementValue { get; init; }
    }

    /// <summary>
    /// An array that should be modified.
    /// </summary>
    [JsonConverter(typeof(PropertyRequestModelConverter))]
    public class ModifyArray : PropertyRequestModel
    {
        /// <summary>
        /// The modification type.
        /// </summary>
        public required ModificationType Type { get; init; }
        /// <summary>
        /// The values to add or remove.
        /// </summary>
        public required List<PropertyLiteral> Values { get; init; }
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

internal class PropertyRequestModelConverter : JsonConverter<PropertyRequestModel>
{
    public override PropertyRequestModel? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        throw new NotImplementedException();

    public override void Write(
        Utf8JsonWriter writer,
        PropertyRequestModel value,
        JsonSerializerOptions options)
    {
        switch (value)
        {
            case PropertyRequestModel.Literal literal:
                JsonSerializer.Serialize(writer, literal.Value, options);
                break;
            case PropertyRequestModel.IncrementInteger inc:
                writer.WriteStartObject();
                writer.WritePropertyName("inc");
                writer.WriteNumberValue(inc.IncrementValue);
                writer.WriteEndObject();
                break;
            case PropertyRequestModel.ModifyArray modify:
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

    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAssignableTo(typeof(PropertyRequestModel));
}

