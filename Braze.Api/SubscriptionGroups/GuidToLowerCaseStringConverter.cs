using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// Explicitly converts Guids to lowercase strings ("D" format).
/// </summary>
internal class GuidToLowerCaseStringConverter : JsonConverter<Guid>
{
    /// <summary>
    /// Reads a Guid from the value or throws an exception if the value is null.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value is null
            ? throw new InvalidOperationException("Value cannot be null.")
            : Guid.Parse(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Writes a Guid as a lowercase string ("D" format).
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("D"));
}
