using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

internal class SubscriptionStateJsonConverter : JsonConverter<SubscriptionState>
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override SubscriptionState Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Enum.Parse<SubscriptionState>(reader.GetString() ?? string.Empty, true);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="subscriptionState"></param>
    /// <param name="options"></param>
    public override void Write(
        Utf8JsonWriter writer,
        SubscriptionState subscriptionState,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(subscriptionState.ToString().ToLowerInvariant());
}
