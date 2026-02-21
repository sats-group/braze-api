using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.SubscriptionGroups;

internal class SubscriptionStateJsonConverter : JsonConverter<SubscriptionGroupSubscribeState>
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override SubscriptionGroupSubscribeState Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
        => reader.TokenType != JsonTokenType.String ? throw new JsonException("Expected a string, got " + reader.TokenType + " instead.") : Enum.Parse<SubscriptionGroupSubscribeState>(reader.GetString() ?? string.Empty, true);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="subscriptionGroupSubscriptionState"></param>
    /// <param name="options"></param>
    public override void Write(
        Utf8JsonWriter writer,
        SubscriptionGroupSubscribeState subscriptionGroupSubscriptionState,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(subscriptionGroupSubscriptionState.ToString().ToLowerInvariant());
}
