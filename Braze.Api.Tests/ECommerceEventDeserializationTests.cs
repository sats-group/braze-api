using System;
using System.Collections.Generic;
using System.Text.Json;
using Braze.Api.UserData;
using Braze.Api.UserData.ECommerce;
using Xunit;

namespace Braze.Api.Tests;

public class ECommerceEventDeserializationTests
{
    [Fact]
    public void ProductViewedEvent_DeserializesCorrectly()
    {
        // Arrange
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""ecommerce.product_viewed"",
            ""time"": ""2024-01-15T09:03:45Z"",
            ""properties"": {
                ""product_id"": ""4111176"",
                ""product_name"": ""Torchie runners"",
                ""variant_id"": ""4111176700"",
                ""price"": 85.0,
                ""currency"": ""GBP"",
                ""source"": ""storefront""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<ProductViewedEvent>(@event);
        var productViewed = (ProductViewedEvent)@event;
        Assert.Equal("user_id", productViewed.ExternalId);
        Assert.Equal("ecommerce.product_viewed", productViewed.Name);
        Assert.Equal("4111176", productViewed.Properties.ProductId);
        Assert.Equal("Torchie runners", productViewed.Properties.ProductName);
        Assert.Equal(85.0m, productViewed.Properties.Price);
        Assert.Equal("GBP", productViewed.Properties.Currency);
    }

    [Fact]
    public void CartUpdatedEvent_DeserializesCorrectly()
    {
        // Arrange
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""ecommerce.cart_updated"",
            ""time"": ""2024-01-15T09:15:30Z"",
            ""properties"": {
                ""cart_id"": ""cart_12345"",
                ""total_value"": 199.98,
                ""currency"": ""USD"",
                ""products"": [
                    {
                        ""product_id"": ""8266836345064"",
                        ""product_name"": ""Classic T-Shirt"",
                        ""variant_id"": ""44610569208040"",
                        ""quantity"": 2,
                        ""price"": 99.99
                    }
                ],
                ""source"": ""storefront""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<CartUpdatedEvent>(@event);
        var cartUpdated = (CartUpdatedEvent)@event;
        Assert.Equal("cart_12345", cartUpdated.Properties.CartId);
        Assert.Equal(199.98m, cartUpdated.Properties.TotalValue);
        Assert.Equal("USD", cartUpdated.Properties.Currency);
        Assert.Single(cartUpdated.Properties.Products);
    }

    [Fact]
    public void CheckoutStartedEvent_DeserializesCorrectly()
    {
        // Arrange
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""ecommerce.checkout_started"",
            ""time"": ""2024-01-15T09:25:45Z"",
            ""properties"": {
                ""checkout_id"": ""checkout_abc123"",
                ""cart_id"": ""cart_12345"",
                ""total_value"": 199.98,
                ""currency"": ""USD"",
                ""products"": [
                    {
                        ""product_id"": ""632910392"",
                        ""product_name"": ""Wireless Headphones"",
                        ""variant_id"": ""808950810"",
                        ""quantity"": 1,
                        ""price"": 199.98
                    }
                ],
                ""source"": ""storefront""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<CheckoutStartedEvent>(@event);
        var checkoutStarted = (CheckoutStartedEvent)@event;
        Assert.Equal("checkout_abc123", checkoutStarted.Properties.CheckoutId);
        Assert.Equal("cart_12345", checkoutStarted.Properties.CartId);
        Assert.Equal(199.98m, checkoutStarted.Properties.TotalValue);
    }

    [Fact]
    public void OrderPlacedEvent_DeserializesCorrectly()
    {
        // Arrange
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""ecommerce.order_placed"",
            ""time"": ""2024-01-15T09:35:20Z"",
            ""properties"": {
                ""order_id"": ""order_67890"",
                ""cart_id"": ""cart_12345"",
                ""total_value"": 189.98,
                ""currency"": ""USD"",
                ""total_discounts"": 10.00,
                ""discounts"": [
                    {
                        ""code"": ""SAVE10"",
                        ""amount"": 10.00
                    }
                ],
                ""products"": [
                    {
                        ""product_id"": ""632910392"",
                        ""product_name"": ""Wireless Headphones"",
                        ""variant_id"": ""808950810"",
                        ""quantity"": 1,
                        ""price"": 199.98
                    }
                ],
                ""source"": ""storefront""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<OrderPlacedEvent>(@event);
        var orderPlaced = (OrderPlacedEvent)@event;
        Assert.Equal("order_67890", orderPlaced.Properties.OrderId);
        Assert.Equal(189.98m, orderPlaced.Properties.TotalValue);
        Assert.Equal(10.00m, orderPlaced.Properties.TotalDiscounts);
        Assert.NotNull(orderPlaced.Properties.Discounts);
        Assert.Single(orderPlaced.Properties.Discounts);
    }

    [Fact]
    public void OrderRefundedEvent_DeserializesCorrectly()
    {
        // Arrange
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""ecommerce.order_refunded"",
            ""time"": ""2024-01-15T10:15:30Z"",
            ""properties"": {
                ""order_id"": ""order_67890"",
                ""total_value"": 99.99,
                ""currency"": ""USD"",
                ""products"": [
                    {
                        ""product_id"": ""632910392"",
                        ""product_name"": ""Wireless Headphones"",
                        ""variant_id"": ""808950810"",
                        ""quantity"": 1,
                        ""price"": 99.99
                    }
                ],
                ""source"": ""storefront""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<OrderRefundedEvent>(@event);
        var orderRefunded = (OrderRefundedEvent)@event;
        Assert.Equal("order_67890", orderRefunded.Properties.OrderId);
        Assert.Equal(99.99m, orderRefunded.Properties.TotalValue);
        Assert.Equal("USD", orderRefunded.Properties.Currency);
    }

    [Fact]
    public void OrderCancelledEvent_DeserializesCorrectly()
    {
        // Arrange
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""ecommerce.order_cancelled"",
            ""time"": ""2024-01-15T10:45:15Z"",
            ""properties"": {
                ""order_id"": ""order_67890"",
                ""cancel_reason"": ""customer changed mind"",
                ""total_value"": 189.98,
                ""currency"": ""USD"",
                ""products"": [
                    {
                        ""product_id"": ""632910392"",
                        ""product_name"": ""Wireless Headphones"",
                        ""variant_id"": ""808950810"",
                        ""quantity"": 1,
                        ""price"": 199.98
                    }
                ],
                ""source"": ""storefront""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<OrderCancelledEvent>(@event);
        var orderCancelled = (OrderCancelledEvent)@event;
        Assert.Equal("order_67890", orderCancelled.Properties.OrderId);
        Assert.Equal("customer changed mind", orderCancelled.Properties.CancelReason);
        Assert.Equal(189.98m, orderCancelled.Properties.TotalValue);
    }

    [Fact]
    public void CustomEvent_DeserializesCorrectly()
    {
        // Arrange - Test that non-ecommerce events deserialize as CustomEvent
        var json = @"{
            ""external_id"": ""user_id"",
            ""name"": ""my_custom_event"",
            ""time"": ""2024-01-15T10:00:00Z"",
            ""properties"": {
                ""my_property"": ""my_value""
            }
        }";

        // Act
        var @event = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(@event);
        Assert.IsType<CustomEvent>(@event);
        var customEvent = (CustomEvent)@event;
        Assert.Equal("my_custom_event", customEvent.Name);
        Assert.NotNull(customEvent.Properties);
    }

    [Fact]
    public void SerializeAndDeserialize_RoundTrip()
    {
        // Arrange
        var original = new ProductViewedEvent
        {
            ExternalId = "user_123",
            Time = DateTimeOffset.Parse("2024-01-15T10:00:00Z"),
            Properties = new ProductViewedProperties
            {
                ProductId = "prod_001",
                ProductName = "Test Product",
                VariantId = "var_001",
                Price = 29.99m,
                Currency = "EUR",
                Source = "web"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(original, DefaultJsonSerializerOptions.Options);
        var deserialized = JsonSerializer.Deserialize<Event>(json, DefaultJsonSerializerOptions.Options);

        // Assert
        Assert.NotNull(deserialized);
        Assert.IsType<ProductViewedEvent>(deserialized);
        var productViewed = (ProductViewedEvent)deserialized;
        Assert.Equal(original.ExternalId, productViewed.ExternalId);
        Assert.Equal(original.Properties.ProductId, productViewed.Properties.ProductId);
        Assert.Equal(original.Properties.Price, productViewed.Properties.Price);
    }
}
