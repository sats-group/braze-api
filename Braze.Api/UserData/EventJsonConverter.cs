using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Braze.Api.UserData.ECommerce;

namespace Braze.Api.UserData;

/// <summary>
/// JSON converter for Event that uses the 'name' property as a type discriminator.
/// </summary>
public class EventJsonConverter : JsonConverter<Event>
{
    /// <summary>
    /// Reads and deserializes JSON into an Event object, using the 'name' property as a type discriminator.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader to read from.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The deserialized Event object.</returns>
    public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON into a JsonDocument to peek at the 'name' property
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Check if 'name' property exists
        if (!root.TryGetProperty("name", out var nameProperty))
        {
            throw new JsonException("Event JSON must have a 'name' property");
        }

        var name = nameProperty.GetString();
        if (string.IsNullOrEmpty(name))
        {
            throw new JsonException("Event 'name' property cannot be null or empty");
        }

        // Determine the concrete type based on the name
        Type concreteType = name switch
        {
            "ecommerce.product_viewed" => typeof(ProductViewedEvent),
            "ecommerce.cart_updated" => typeof(CartUpdatedEvent),
            "ecommerce.checkout_started" => typeof(CheckoutStartedEvent),
            "ecommerce.order_placed" => typeof(OrderPlacedEvent),
            "ecommerce.order_refunded" => typeof(OrderRefundedEvent),
            "ecommerce.order_cancelled" => typeof(OrderCancelledEvent),
            _ => typeof(CustomEvent) // Default to CustomEvent for non-ecommerce events
        };

        // Create new options without the converter to avoid infinite recursion
        var optionsWithoutConverter = new JsonSerializerOptions(options);
        optionsWithoutConverter.Converters.Clear();
        foreach (var converter in options.Converters)
        {
            if (converter.GetType() != typeof(EventJsonConverter))
            {
                optionsWithoutConverter.Converters.Add(converter);
            }
        }

        // Deserialize to the concrete type
        var json = root.GetRawText();
        var result = JsonSerializer.Deserialize(json, concreteType, optionsWithoutConverter);
        
        if (result == null)
        {
            throw new JsonException($"Failed to deserialize Event with name '{name}'");
        }

        return (Event)result;
    }

    /// <summary>
    /// Writes an Event object to JSON.
    /// </summary>
    /// <param name="writer">The Utf8JsonWriter to write to.</param>
    /// <param name="value">The Event object to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
    {
        // Create new options without the converter to avoid infinite recursion
        var optionsWithoutConverter = new JsonSerializerOptions(options);
        optionsWithoutConverter.Converters.Clear();
        foreach (var converter in options.Converters)
        {
            if (converter.GetType() != typeof(EventJsonConverter))
            {
                optionsWithoutConverter.Converters.Add(converter);
            }
        }

        // Serialize using the concrete type
        JsonSerializer.Serialize(writer, value, value.GetType(), optionsWithoutConverter);
    }
}
