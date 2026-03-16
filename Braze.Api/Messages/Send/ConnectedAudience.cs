using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Represents a Connected Audience Object for filtering recipients of a triggered message.
/// Can be a logical compound (<see cref="And"/>/<see cref="Or"/>) or a single filter
/// (e.g. <see cref="SegmentFilter"/>, <see cref="CustomAttributeFilter"/>).
/// See https://www.braze.com/docs/api/objects_filters/connected_audience/
/// </summary>
[JsonConverter(typeof(ConnectedAudienceConverter))]
public abstract class ConnectedAudience
{
    private ConnectedAudience()
    {
    }

    /// <summary>
    /// Creates an AND compound: all filters must match for a user to be included.
    /// </summary>
    /// <param name="filters">The audience filters to combine with AND logic.</param>
    /// <returns>An AND compound audience.</returns>
    public static ConnectedAudience CreateAnd(IEnumerable<ConnectedAudience> filters) =>
        new And { Filters = filters };

    /// <summary>
    /// Creates an OR compound: at least one filter must match for a user to be included.
    /// </summary>
    /// <param name="filters">The audience filters to combine with OR logic.</param>
    /// <returns>An OR compound audience.</returns>
    public static ConnectedAudience CreateOr(IEnumerable<ConnectedAudience> filters) =>
        new Or { Filters = filters };

    /// <summary>
    /// AND compound of multiple audience filters.
    /// Serializes as <c>{"AND": [...]}</c>.
    /// </summary>
    public sealed class And : ConnectedAudience
    {
        /// <summary>
        /// The audience filters combined with AND logic.
        /// </summary>
        public required IEnumerable<ConnectedAudience> Filters { get; init; }
    }

    /// <summary>
    /// OR compound of multiple audience filters.
    /// Serializes as <c>{"OR": [...]}</c>.
    /// </summary>
    public sealed class Or : ConnectedAudience
    {
        /// <summary>
        /// The audience filters combined with OR logic.
        /// </summary>
        public required IEnumerable<ConnectedAudience> Filters { get; init; }
    }

    /// <summary>
    /// Segment membership filter.
    /// Serializes as <c>{"segment": {"segment_id": "..."}}</c>.
    /// </summary>
    public sealed class SegmentFilter : ConnectedAudience
    {
        /// <summary>
        /// The segment ID to filter by.
        /// </summary>
        [JsonPropertyName("segment_id")]
        public required string SegmentId { get; init; }
    }

    /// <summary>
    /// Custom attribute filter.
    /// Serializes as <c>{"custom_attribute": {"custom_attribute_definition_id": "...", "comparison": "...", "value": ...}}</c>.
    /// </summary>
    public sealed class CustomAttributeFilter : ConnectedAudience
    {
        /// <summary>
        /// The name/ID of the custom attribute to filter by.
        /// </summary>
        [JsonPropertyName("custom_attribute_definition_id")]
        public required string CustomAttributeDefinitionId { get; init; }

        /// <summary>
        /// The comparison operator (e.g., <c>"equals"</c>, <c>"not_equal"</c>, <c>"greater_than"</c>, <c>"contains"</c>).
        /// </summary>
        [JsonPropertyName("comparison")]
        public required string Comparison { get; init; }

        /// <summary>
        /// The value to compare against.
        /// </summary>
        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Property? Value { get; init; }
    }

    /// <summary>
    /// Push subscription status filter.
    /// Serializes as <c>{"push_subscription_status": {"comparison": "...", "value": "..."}}</c>.
    /// </summary>
    public sealed class PushSubscriptionStatusFilter : ConnectedAudience
    {
        /// <summary>
        /// The comparison operator (e.g., <c>"is"</c>).
        /// </summary>
        [JsonPropertyName("comparison")]
        public required string Comparison { get; init; }

        /// <summary>
        /// The subscription status value (e.g., <c>"opted_in"</c>, <c>"subscribed"</c>, <c>"unsubscribed"</c>).
        /// </summary>
        [JsonPropertyName("value")]
        public required string Value { get; init; }
    }

    /// <summary>
    /// Email subscription status filter.
    /// Serializes as <c>{"email_subscription_status": {"comparison": "...", "value": "..."}}</c>.
    /// </summary>
    public sealed class EmailSubscriptionStatusFilter : ConnectedAudience
    {
        /// <summary>
        /// The comparison operator (e.g., <c>"is"</c>).
        /// </summary>
        [JsonPropertyName("comparison")]
        public required string Comparison { get; init; }

        /// <summary>
        /// The subscription status value (e.g., <c>"opted_in"</c>, <c>"subscribed"</c>, <c>"unsubscribed"</c>).
        /// </summary>
        [JsonPropertyName("value")]
        public required string Value { get; init; }
    }
}

internal class ConnectedAudienceConverter : JsonConverter<ConnectedAudience>
{
    public override ConnectedAudience? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException($"Expected JSON object for ConnectedAudience, got {root.ValueKind}.");
        }

        if (root.TryGetProperty("AND", out var andProp))
        {
            var filters = andProp.EnumerateArray()
                .Select(e => e.Deserialize<ConnectedAudience>(options))
                .OfType<ConnectedAudience>()
                .ToList();
            return new ConnectedAudience.And { Filters = filters };
        }

        if (root.TryGetProperty("OR", out var orProp))
        {
            var filters = orProp.EnumerateArray()
                .Select(e => e.Deserialize<ConnectedAudience>(options))
                .OfType<ConnectedAudience>()
                .ToList();
            return new ConnectedAudience.Or { Filters = filters };
        }

        if (root.TryGetProperty("segment", out var segmentProp))
        {
            return new ConnectedAudience.SegmentFilter
            {
                SegmentId = segmentProp.GetProperty("segment_id").GetString()
                    ?? throw new JsonException("Missing 'segment_id' in segment filter."),
            };
        }

        if (root.TryGetProperty("custom_attribute", out var customAttrProp))
        {
            return new ConnectedAudience.CustomAttributeFilter
            {
                CustomAttributeDefinitionId = customAttrProp.GetProperty("custom_attribute_definition_id").GetString()
                    ?? throw new JsonException("Missing 'custom_attribute_definition_id' in custom_attribute filter."),
                Comparison = customAttrProp.GetProperty("comparison").GetString()
                    ?? throw new JsonException("Missing 'comparison' in custom_attribute filter."),
                Value = customAttrProp.TryGetProperty("value", out var valueProp)
                    ? valueProp.Deserialize<Property>(options)
                    : null,
            };
        }

        if (root.TryGetProperty("push_subscription_status", out var pushProp))
        {
            return new ConnectedAudience.PushSubscriptionStatusFilter
            {
                Comparison = pushProp.GetProperty("comparison").GetString()
                    ?? throw new JsonException("Missing 'comparison' in push_subscription_status filter."),
                Value = pushProp.GetProperty("value").GetString()
                    ?? throw new JsonException("Missing 'value' in push_subscription_status filter."),
            };
        }

        if (root.TryGetProperty("email_subscription_status", out var emailProp))
        {
            return new ConnectedAudience.EmailSubscriptionStatusFilter
            {
                Comparison = emailProp.GetProperty("comparison").GetString()
                    ?? throw new JsonException("Missing 'comparison' in email_subscription_status filter."),
                Value = emailProp.GetProperty("value").GetString()
                    ?? throw new JsonException("Missing 'value' in email_subscription_status filter."),
            };
        }

        throw new JsonException("Unknown or unsupported ConnectedAudience filter type.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        ConnectedAudience value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        switch (value)
        {
            case ConnectedAudience.And and:
                writer.WritePropertyName("AND");
                writer.WriteStartArray();
                foreach (var filter in and.Filters)
                {
                    JsonSerializer.Serialize(writer, filter, options);
                }

                writer.WriteEndArray();
                break;

            case ConnectedAudience.Or or:
                writer.WritePropertyName("OR");
                writer.WriteStartArray();
                foreach (var filter in or.Filters)
                {
                    JsonSerializer.Serialize(writer, filter, options);
                }

                writer.WriteEndArray();
                break;

            case ConnectedAudience.SegmentFilter segment:
                writer.WritePropertyName("segment");
                writer.WriteStartObject();
                writer.WriteString("segment_id", segment.SegmentId);
                writer.WriteEndObject();
                break;

            case ConnectedAudience.CustomAttributeFilter customAttr:
                writer.WritePropertyName("custom_attribute");
                writer.WriteStartObject();
                writer.WriteString("custom_attribute_definition_id", customAttr.CustomAttributeDefinitionId);
                writer.WriteString("comparison", customAttr.Comparison);
                if (customAttr.Value != null)
                {
                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, customAttr.Value, options);
                }

                writer.WriteEndObject();
                break;

            case ConnectedAudience.PushSubscriptionStatusFilter push:
                writer.WritePropertyName("push_subscription_status");
                writer.WriteStartObject();
                writer.WriteString("comparison", push.Comparison);
                writer.WriteString("value", push.Value);
                writer.WriteEndObject();
                break;

            case ConnectedAudience.EmailSubscriptionStatusFilter email:
                writer.WritePropertyName("email_subscription_status");
                writer.WriteStartObject();
                writer.WriteString("comparison", email.Comparison);
                writer.WriteString("value", email.Value);
                writer.WriteEndObject();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(value), $"Unknown ConnectedAudience type: {value.GetType().Name}");
        }

        writer.WriteEndObject();
    }

    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsAssignableTo(typeof(ConnectedAudience));
}
