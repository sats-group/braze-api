using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Braze.Api.UserData.ECommerce;
using Xunit;

namespace Braze.Api.Tests;

public class ECommerceEventSerializationTests
{
    [Fact]
    public void OrderPlacedEvent_SerializesCorrectly_WithAllProperties()
    {
        // Arrange - Using the exact example from Braze documentation
        var orderPlaced = new OrderPlacedEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new OrderPlacedProperties
            {
                OrderId = "order_67890",
                CartId = "cart_12345",
                TotalValue = 189.98m,
                Currency = "USD",
                TotalDiscounts = 10.00m,
                Discounts = new List<Discount>
                {
                    new()
                    {
                        Code = "SAVE10",
                        Amount = 10.00m
                    }
                },
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "632910392",
                        ProductName = "Wireless Headphones",
                        VariantId = "808950810",
                        Quantity = 1,
                        Price = 199.98m,
                        Metadata = new Dictionary<string, object>
                        {
                            { "sku", "WH-BLK-PRO" },
                            { "color", "Black" },
                            { "brand", "BrazeAudio" }
                        }
                    }
                },
                Source = "https://braze-audio.com",
                Metadata = new Dictionary<string, object>
                {
                    { "order_status_url", "https://braze-audio.com/orders/67890/status" },
                    { "order_number", "ORD-2024-001234" },
                    { "tags", new List<string> { "electronics", "audio" } },
                    { "referring_site", "https://www.e-referrals.com" },
                    { "payment_gateway_names", new List<string> { "tap2pay", "dotcash" } }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderPlaced, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify top-level event properties
        Assert.Equal("user_id", root.GetProperty("external_id").GetString());
        Assert.Equal("your_app_identifier", root.GetProperty("app_id").GetString());
        Assert.Equal("ecommerce.order_placed", root.GetProperty("name").GetString());
        // DateTimeOffset serializes as ISO-8601 with offset (e.g., "+00:00") which is equivalent to "Z"
        Assert.True(root.TryGetProperty("time", out var timeProperty));
        var timeString = timeProperty.GetString();
        Assert.NotNull(timeString);
        Assert.Contains("2024-01-15T09:35:20", timeString);

        // Assert - Verify properties object exists
        Assert.True(root.TryGetProperty("properties", out var properties));

        // Assert - Verify order-level properties
        Assert.Equal("order_67890", properties.GetProperty("order_id").GetString());
        Assert.Equal("cart_12345", properties.GetProperty("cart_id").GetString());
        Assert.Equal(189.98m, properties.GetProperty("total_value").GetDecimal());
        Assert.Equal("USD", properties.GetProperty("currency").GetString());
        Assert.Equal(10.00m, properties.GetProperty("total_discounts").GetDecimal());
        Assert.Equal("https://braze-audio.com", properties.GetProperty("source").GetString());

        // Assert - Verify discounts array
        var discounts = properties.GetProperty("discounts");
        Assert.Equal(JsonValueKind.Array, discounts.ValueKind);
        Assert.Single(discounts.EnumerateArray());
        var discount = discounts[0];
        Assert.Equal("SAVE10", discount.GetProperty("code").GetString());
        Assert.Equal(10.00m, discount.GetProperty("amount").GetDecimal());

        // Assert - Verify products array
        var products = properties.GetProperty("products");
        Assert.Equal(JsonValueKind.Array, products.ValueKind);
        Assert.Single(products.EnumerateArray());
        var product = products[0];
        Assert.Equal("632910392", product.GetProperty("product_id").GetString());
        Assert.Equal("Wireless Headphones", product.GetProperty("product_name").GetString());
        Assert.Equal("808950810", product.GetProperty("variant_id").GetString());
        Assert.Equal(1, product.GetProperty("quantity").GetInt32());
        Assert.Equal(199.98m, product.GetProperty("price").GetDecimal());

        // Assert - Verify product metadata
        var productMetadata = product.GetProperty("metadata");
        Assert.Equal("WH-BLK-PRO", productMetadata.GetProperty("sku").GetString());
        Assert.Equal("Black", productMetadata.GetProperty("color").GetString());
        Assert.Equal("BrazeAudio", productMetadata.GetProperty("brand").GetString());

        // Assert - Verify order metadata
        var metadata = properties.GetProperty("metadata");
        Assert.Equal("https://braze-audio.com/orders/67890/status", metadata.GetProperty("order_status_url").GetString());
        Assert.Equal("ORD-2024-001234", metadata.GetProperty("order_number").GetString());
        Assert.Equal("https://www.e-referrals.com", metadata.GetProperty("referring_site").GetString());

        // Assert - Verify metadata arrays
        var tags = metadata.GetProperty("tags");
        Assert.Equal(JsonValueKind.Array, tags.ValueKind);
        Assert.Equal(2, tags.GetArrayLength());
        Assert.Equal("electronics", tags[0].GetString());
        Assert.Equal("audio", tags[1].GetString());

        var paymentGateways = metadata.GetProperty("payment_gateway_names");
        Assert.Equal(JsonValueKind.Array, paymentGateways.ValueKind);
        Assert.Equal(2, paymentGateways.GetArrayLength());
        Assert.Equal("tap2pay", paymentGateways[0].GetString());
        Assert.Equal("dotcash", paymentGateways[1].GetString());
    }

    [Fact]
    public void OrderPlacedEvent_SerializesCorrectly_WithMinimalProperties()
    {
        // Arrange - Test with only required fields
        var orderPlaced = new OrderPlacedEvent
        {
            ExternalId = "user_123",
            Time = DateTimeOffset.Parse("2024-01-15T10:00:00Z"),
            Properties = new OrderPlacedProperties
            {
                OrderId = "order_minimal",
                TotalValue = 99.99m,
                Currency = "EUR",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "prod_001",
                        ProductName = "Test Product",
                        VariantId = "var_001",
                        Quantity = 1,
                        Price = 99.99m
                    }
                },
                Source = "web"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderPlaced, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify required fields are present
        Assert.Equal("user_123", root.GetProperty("external_id").GetString());
        Assert.Equal("ecommerce.order_placed", root.GetProperty("name").GetString());
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("order_minimal", properties.GetProperty("order_id").GetString());
        Assert.Equal(99.99m, properties.GetProperty("total_value").GetDecimal());
        Assert.Equal("EUR", properties.GetProperty("currency").GetString());

        // Assert - Verify optional fields are omitted when null
        Assert.False(properties.TryGetProperty("cart_id", out _));
        Assert.False(properties.TryGetProperty("total_discounts", out _));
        Assert.False(properties.TryGetProperty("discounts", out _));
        Assert.False(properties.TryGetProperty("metadata", out _));

        // Assert - Verify product has no optional fields
        var product = properties.GetProperty("products")[0];
        Assert.False(product.TryGetProperty("image_url", out _));
        Assert.False(product.TryGetProperty("product_url", out _));
        Assert.False(product.TryGetProperty("metadata", out _));
    }

    [Fact]
    public void OrderPlacedEvent_SerializesCorrectly_WithProductUrls()
    {
        // Arrange - Test optional URL fields
        var orderPlaced = new OrderPlacedEvent
        {
            ExternalId = "user_456",
            Time = DateTimeOffset.UtcNow,
            Properties = new OrderPlacedProperties
            {
                OrderId = "order_urls",
                TotalValue = 49.99m,
                Currency = "GBP",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "prod_002",
                        ProductName = "Product With URLs",
                        VariantId = "var_002",
                        Quantity = 1,
                        Price = 49.99m,
                        ImageUrl = new Uri("https://example.com/image.jpg"),
                        ProductUrl = new Uri("https://example.com/product/002")
                    }
                },
                Source = "mobile_app"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderPlaced, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify URLs are serialized
        var product = root.GetProperty("properties").GetProperty("products")[0];
        Assert.Equal("https://example.com/image.jpg", product.GetProperty("image_url").GetString());
        Assert.Equal("https://example.com/product/002", product.GetProperty("product_url").GetString());
    }

    [Fact]
    public void OrderPlacedEvent_SerializesCorrectly_WithMultipleProducts()
    {
        // Arrange - Test with multiple products and discounts
        var orderPlaced = new OrderPlacedEvent
        {
            ExternalId = "user_789",
            Time = DateTimeOffset.UtcNow,
            Properties = new OrderPlacedProperties
            {
                OrderId = "order_multi",
                TotalValue = 299.97m,
                Currency = "USD",
                TotalDiscounts = 30.00m,
                Discounts = new List<Discount>
                {
                    new() { Code = "WELCOME", Amount = 20.00m },
                    new() { Code = "LOYALTY", Amount = 10.00m }
                },
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "prod_a",
                        ProductName = "Product A",
                        VariantId = "var_a",
                        Quantity = 2,
                        Price = 99.99m
                    },
                    new()
                    {
                        ProductId = "prod_b",
                        ProductName = "Product B",
                        VariantId = "var_b",
                        Quantity = 1,
                        Price = 99.99m
                    }
                },
                Source = "web"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderPlaced, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var properties = jsonDoc.RootElement.GetProperty("properties");

        // Assert - Verify multiple discounts
        var discounts = properties.GetProperty("discounts");
        Assert.Equal(2, discounts.GetArrayLength());
        Assert.Equal("WELCOME", discounts[0].GetProperty("code").GetString());
        Assert.Equal(20.00m, discounts[0].GetProperty("amount").GetDecimal());
        Assert.Equal("LOYALTY", discounts[1].GetProperty("code").GetString());
        Assert.Equal(10.00m, discounts[1].GetProperty("amount").GetDecimal());

        // Assert - Verify multiple products
        var products = properties.GetProperty("products");
        Assert.Equal(2, products.GetArrayLength());
        Assert.Equal("prod_a", products[0].GetProperty("product_id").GetString());
        Assert.Equal(2, products[0].GetProperty("quantity").GetInt32());
        Assert.Equal("prod_b", products[1].GetProperty("product_id").GetString());
        Assert.Equal(1, products[1].GetProperty("quantity").GetInt32());
    }

    [Fact]
    public void OrderPlacedEvent_MatchesBrazeDocumentationFormat()
    {
        // Arrange - Create the exact example from Braze docs
        var orderPlaced = new OrderPlacedEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new OrderPlacedProperties
            {
                OrderId = "order_67890",
                CartId = "cart_12345",
                TotalValue = 189.98m,
                Currency = "USD",
                TotalDiscounts = 10.00m,
                Discounts = new List<Discount>
                {
                    new() { Code = "SAVE10", Amount = 10.00m }
                },
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "632910392",
                        ProductName = "Wireless Headphones",
                        VariantId = "808950810",
                        Quantity = 1,
                        Price = 199.98m,
                        Metadata = new Dictionary<string, object>
                        {
                            { "sku", "WH-BLK-PRO" },
                            { "color", "Black" },
                            { "brand", "BrazeAudio" }
                        }
                    }
                },
                Source = "https://braze-audio.com",
                Metadata = new Dictionary<string, object>
                {
                    { "order_status_url", "https://braze-audio.com/orders/67890/status" },
                    { "order_number", "ORD-2024-001234" },
                    { "tags", new List<string> { "electronics", "audio" } },
                    { "referring_site", "https://www.e-referrals.com" },
                    { "payment_gateway_names", new List<string> { "tap2pay", "dotcash" } }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderPlaced, DefaultJsonSerializerOptions.Options);

        // Expected JSON structure from Braze documentation
        var expectedStructure = """
        {
          "external_id": "user_id",
          "app_id": "your_app_identifier",
          "name": "ecommerce.order_placed",
          "time": "2024-01-15T09:35:20Z",
          "properties": {
            "order_id": "order_67890",
            "cart_id": "cart_12345",
            "total_value": 189.98,
            "currency": "USD",
            "total_discounts": 10.00,
            "discounts": [
              {
                "code": "SAVE10",
                "amount": 10.00
              }
            ],
            "products": [
              {
                "product_id": "632910392",
                "product_name": "Wireless Headphones",
                "variant_id": "808950810",
                "quantity": 1,
                "price": 199.98,
                "metadata": {
                  "sku": "WH-BLK-PRO",
                  "color": "Black",
                  "brand": "BrazeAudio"
                }
              }
            ],
            "source": "https://braze-audio.com",
            "metadata": {
              "order_status_url": "https://braze-audio.com/orders/67890/status",
              "order_number": "ORD-2024-001234",
              "tags": ["electronics", "audio"],
              "referring_site": "https://www.e-referrals.com",
              "payment_gateway_names": ["tap2pay", "dotcash"]
            }
          }
        }
        """;

        // Assert - Parse both and compare structure
        var actualDoc = JsonDocument.Parse(json);
        var expectedDoc = JsonDocument.Parse(expectedStructure);

        // Verify the JSON structure matches (allowing for property order differences)
        var actualRoot = actualDoc.RootElement;
        var expectedRoot = expectedDoc.RootElement;

        AssertJsonElementsEqual(expectedRoot, actualRoot);
    }

    private static void AssertJsonElementsEqual(JsonElement expected, JsonElement actual)
    {
        Assert.Equal(expected.ValueKind, actual.ValueKind);

        switch (expected.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in expected.EnumerateObject())
                {
                    Assert.True(actual.TryGetProperty(property.Name, out var actualValue),
                        $"Property '{property.Name}' not found in actual JSON");
                    AssertJsonElementsEqual(property.Value, actualValue);
                }
                break;

            case JsonValueKind.Array:
                {
                    Assert.Equal(expected.GetArrayLength(), actual.GetArrayLength());
                    using var expectedIterator = expected.EnumerateArray().GetEnumerator();
                    using var actualIterator = actual.EnumerateArray().GetEnumerator();
                    
                    while (expectedIterator.MoveNext() && actualIterator.MoveNext())
                    {
                        AssertJsonElementsEqual(expectedIterator.Current, actualIterator.Current);
                    }
                    break;
                }

            case JsonValueKind.String:
                // For timestamp strings, allow for different ISO-8601 formats (Z vs +00:00)
                var expectedStr = expected.GetString();
                var actualStr = actual.GetString();
                if (expectedStr != null && actualStr != null && 
                    (expectedStr.Contains('Z') || actualStr.Contains('Z') || 
                     expectedStr.Contains('+') || actualStr.Contains('+')))
                {
                    // Try parsing as DateTimeOffset - if both parse, compare as dates
                    if (DateTimeOffset.TryParse(expectedStr, out var expectedDate) &&
                        DateTimeOffset.TryParse(actualStr, out var actualDate))
                    {
                        Assert.Equal(expectedDate, actualDate);
                        break;
                    }
                }
                Assert.Equal(expectedStr, actualStr);
                break;

            case JsonValueKind.Number:
                Assert.Equal(expected.GetDecimal(), actual.GetDecimal());
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                Assert.Equal(expected.GetBoolean(), actual.GetBoolean());
                break;

            case JsonValueKind.Null:
                break;

            default:
                throw new ArgumentException($"Unexpected JSON value kind: {expected.ValueKind}");
        }
    }

    [Fact]
    public void ProductViewedEvent_SerializesCorrectly_WithAllProperties()
    {
        // Arrange
        var productViewed = new ProductViewedEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new ProductViewedProperties
            {
                ProductId = "4111176",
                ProductName = "Torchie runners",
                VariantId = "4111176700",
                Price = 59.99m,
                Currency = "USD",
                Source = "storefront",
                ImageUrl = new Uri("https://braze-apparel.com/images/torchie-runners.jpg"),
                ProductUrl = new Uri("https://braze-apparel.com/footwear-categories/sneakers/torchie-runners"),
                Metadata = new Dictionary<string, object>
                {
                    { "sku", "TR-ORG-42" },
                    { "color", "Orange" }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(productViewed, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify top-level event properties
        Assert.Equal("user_id", root.GetProperty("external_id").GetString());
        Assert.Equal("your_app_identifier", root.GetProperty("app_id").GetString());
        Assert.Equal("ecommerce.product_viewed", root.GetProperty("name").GetString());

        // Assert - Verify properties object
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("4111176", properties.GetProperty("product_id").GetString());
        Assert.Equal("Torchie runners", properties.GetProperty("product_name").GetString());
        Assert.Equal("4111176700", properties.GetProperty("variant_id").GetString());
        Assert.Equal(59.99m, properties.GetProperty("price").GetDecimal());
        Assert.Equal("USD", properties.GetProperty("currency").GetString());
        Assert.Equal("storefront", properties.GetProperty("source").GetString());
        Assert.Equal("https://braze-apparel.com/images/torchie-runners.jpg", properties.GetProperty("image_url").GetString());
        Assert.Equal("https://braze-apparel.com/footwear-categories/sneakers/torchie-runners", properties.GetProperty("product_url").GetString());

        // Assert - Verify metadata
        var metadata = properties.GetProperty("metadata");
        Assert.Equal("TR-ORG-42", metadata.GetProperty("sku").GetString());
        Assert.Equal("Orange", metadata.GetProperty("color").GetString());
    }

    [Fact]
    public void ProductViewedEvent_SerializesCorrectly_WithMinimalProperties()
    {
        // Arrange - Test with only required fields
        var productViewed = new ProductViewedEvent
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
        var json = JsonSerializer.Serialize(productViewed, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify required fields
        Assert.Equal("ecommerce.product_viewed", root.GetProperty("name").GetString());
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("prod_001", properties.GetProperty("product_id").GetString());
        Assert.Equal("Test Product", properties.GetProperty("product_name").GetString());
        Assert.Equal(29.99m, properties.GetProperty("price").GetDecimal());

        // Assert - Verify optional fields are omitted
        Assert.False(properties.TryGetProperty("image_url", out _));
        Assert.False(properties.TryGetProperty("product_url", out _));
        Assert.False(properties.TryGetProperty("metadata", out _));
    }

    [Fact]
    public void CartUpdatedEvent_SerializesCorrectly_WithAllProperties()
    {
        // Arrange
        var cartUpdated = new CartUpdatedEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new CartUpdatedProperties
            {
                CartId = "CART-8675309",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "4111176",
                        ProductName = "Torchie runners",
                        VariantId = "4111176700",
                        Quantity = 2,
                        Price = 129.99m
                    },
                    new()
                    {
                        ProductId = "5123343",
                        ProductName = "Jetset backpack",
                        VariantId = "5123343991",
                        Quantity = 1,
                        Price = 79.99m
                    }
                },
                Source = "storefront",
                Metadata = new Dictionary<string, object>
                {
                    { "session_id", "sess_abc123" }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(cartUpdated, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify top-level event properties
        Assert.Equal("ecommerce.cart_updated", root.GetProperty("name").GetString());

        // Assert - Verify properties
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("CART-8675309", properties.GetProperty("cart_id").GetString());
        Assert.Equal("storefront", properties.GetProperty("source").GetString());

        // Assert - Verify products array
        var products = properties.GetProperty("products");
        Assert.Equal(JsonValueKind.Array, products.ValueKind);
        Assert.Equal(2, products.GetArrayLength());
        Assert.Equal("4111176", products[0].GetProperty("product_id").GetString());
        Assert.Equal("Torchie runners", products[0].GetProperty("product_name").GetString());
        Assert.Equal(2, products[0].GetProperty("quantity").GetInt32());
        Assert.Equal(129.99m, products[0].GetProperty("price").GetDecimal());
        Assert.Equal("5123343", products[1].GetProperty("product_id").GetString());

        // Assert - Verify metadata
        var metadata = properties.GetProperty("metadata");
        Assert.Equal("sess_abc123", metadata.GetProperty("session_id").GetString());
    }

    [Fact]
    public void CartUpdatedEvent_SerializesCorrectly_WithMinimalProperties()
    {
        // Arrange - Test with only required fields
        var cartUpdated = new CartUpdatedEvent
        {
            ExternalId = "user_123",
            Time = DateTimeOffset.Parse("2024-01-15T10:00:00Z"),
            Properties = new CartUpdatedProperties
            {
                CartId = "cart_minimal",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "prod_001",
                        ProductName = "Test Product",
                        VariantId = "var_001",
                        Quantity = 1,
                        Price = 49.99m
                    }
                },
                Source = "web"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(cartUpdated, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify required fields
        Assert.Equal("ecommerce.cart_updated", root.GetProperty("name").GetString());
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("cart_minimal", properties.GetProperty("cart_id").GetString());

        // Assert - Verify optional metadata is omitted
        Assert.False(properties.TryGetProperty("metadata", out _));
    }

    [Fact]
    public void CheckoutStartedEvent_SerializesCorrectly_WithAllProperties()
    {
        // Arrange
        var checkoutStarted = new CheckoutStartedEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new CheckoutStartedProperties
            {
                CheckoutId = "CHECKOUT-321",
                CartId = "CART-8675309",
                TotalValue = 339.97m,
                Currency = "USD",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "4111176",
                        ProductName = "Torchie runners",
                        VariantId = "4111176700",
                        Quantity = 2,
                        Price = 129.99m
                    }
                },
                Source = "storefront",
                Metadata = new Dictionary<string, object>
                {
                    { "coupon_code", "SAVE10" }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(checkoutStarted, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify top-level event properties
        Assert.Equal("ecommerce.checkout_started", root.GetProperty("name").GetString());

        // Assert - Verify properties
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("CHECKOUT-321", properties.GetProperty("checkout_id").GetString());
        Assert.Equal("CART-8675309", properties.GetProperty("cart_id").GetString());
        Assert.Equal(339.97m, properties.GetProperty("total_value").GetDecimal());
        Assert.Equal("USD", properties.GetProperty("currency").GetString());
        Assert.Equal("storefront", properties.GetProperty("source").GetString());

        // Assert - Verify products array
        var products = properties.GetProperty("products");
        Assert.Equal(JsonValueKind.Array, products.ValueKind);
        Assert.Single(products.EnumerateArray());
        Assert.Equal("4111176", products[0].GetProperty("product_id").GetString());

        // Assert - Verify metadata
        var metadata = properties.GetProperty("metadata");
        Assert.Equal("SAVE10", metadata.GetProperty("coupon_code").GetString());
    }

    [Fact]
    public void CheckoutStartedEvent_SerializesCorrectly_WithMinimalProperties()
    {
        // Arrange - Test with only required fields (no cart_id)
        var checkoutStarted = new CheckoutStartedEvent
        {
            ExternalId = "user_123",
            Time = DateTimeOffset.Parse("2024-01-15T10:00:00Z"),
            Properties = new CheckoutStartedProperties
            {
                CheckoutId = "checkout_minimal",
                TotalValue = 99.99m,
                Currency = "GBP",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "prod_001",
                        ProductName = "Test Product",
                        VariantId = "var_001",
                        Quantity = 1,
                        Price = 99.99m
                    }
                },
                Source = "mobile_app"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(checkoutStarted, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify required fields
        Assert.Equal("ecommerce.checkout_started", root.GetProperty("name").GetString());
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("checkout_minimal", properties.GetProperty("checkout_id").GetString());

        // Assert - Verify optional fields are omitted
        Assert.False(properties.TryGetProperty("cart_id", out _));
        Assert.False(properties.TryGetProperty("metadata", out _));
    }

    [Fact]
    public void OrderRefundedEvent_SerializesCorrectly_WithAllProperties()
    {
        // Arrange
        var orderRefunded = new OrderRefundedEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new OrderRefundedProperties
            {
                OrderId = "order_67890",
                RefundId = "refund_11111",
                RefundAmount = 99.99m,
                Currency = "USD",
                Source = "storefront",
                RefundReason = "Defective item",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "632910392",
                        ProductName = "Wireless Headphones",
                        VariantId = "808950810",
                        Quantity = 1,
                        Price = 99.99m
                    }
                },
                Metadata = new Dictionary<string, object>
                {
                    { "refund_status_url", "https://example.com/refunds/11111" }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderRefunded, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify top-level event properties
        Assert.Equal("ecommerce.order_refunded", root.GetProperty("name").GetString());

        // Assert - Verify properties
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("order_67890", properties.GetProperty("order_id").GetString());
        Assert.Equal("refund_11111", properties.GetProperty("refund_id").GetString());
        Assert.Equal(99.99m, properties.GetProperty("refund_amount").GetDecimal());
        Assert.Equal("USD", properties.GetProperty("currency").GetString());
        Assert.Equal("storefront", properties.GetProperty("source").GetString());
        Assert.Equal("Defective item", properties.GetProperty("refund_reason").GetString());

        // Assert - Verify products array
        var products = properties.GetProperty("products");
        Assert.Equal(JsonValueKind.Array, products.ValueKind);
        Assert.Single(products.EnumerateArray());
        Assert.Equal("632910392", products[0].GetProperty("product_id").GetString());

        // Assert - Verify metadata
        var metadata = properties.GetProperty("metadata");
        Assert.Equal("https://example.com/refunds/11111", metadata.GetProperty("refund_status_url").GetString());
    }

    [Fact]
    public void OrderRefundedEvent_SerializesCorrectly_WithMinimalProperties()
    {
        // Arrange - Test with only required fields
        var orderRefunded = new OrderRefundedEvent
        {
            ExternalId = "user_123",
            Time = DateTimeOffset.Parse("2024-01-15T10:00:00Z"),
            Properties = new OrderRefundedProperties
            {
                OrderId = "order_minimal",
                RefundId = "refund_minimal",
                RefundAmount = 49.99m,
                Currency = "EUR",
                Source = "web"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderRefunded, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify required fields
        Assert.Equal("ecommerce.order_refunded", root.GetProperty("name").GetString());
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("order_minimal", properties.GetProperty("order_id").GetString());
        Assert.Equal("refund_minimal", properties.GetProperty("refund_id").GetString());
        Assert.Equal(49.99m, properties.GetProperty("refund_amount").GetDecimal());

        // Assert - Verify optional fields are omitted
        Assert.False(properties.TryGetProperty("refund_reason", out _));
        Assert.False(properties.TryGetProperty("products", out _));
        Assert.False(properties.TryGetProperty("metadata", out _));
    }

    [Fact]
    public void OrderCancelledEvent_SerializesCorrectly_WithAllProperties()
    {
        // Arrange
        var orderCancelled = new OrderCancelledEvent
        {
            ExternalId = "user_id",
            AppId = "your_app_identifier",
            Time = DateTimeOffset.Parse("2024-01-15T09:35:20Z"),
            Properties = new OrderCancelledProperties
            {
                OrderId = "order_67890",
                Currency = "USD",
                Source = "storefront",
                CancelReason = "Customer request",
                Products = new List<Product>
                {
                    new()
                    {
                        ProductId = "632910392",
                        ProductName = "Wireless Headphones",
                        VariantId = "808950810",
                        Quantity = 1,
                        Price = 199.98m
                    }
                },
                Metadata = new Dictionary<string, object>
                {
                    { "cancellation_url", "https://example.com/orders/67890/cancel" }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderCancelled, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify top-level event properties
        Assert.Equal("ecommerce.order_cancelled", root.GetProperty("name").GetString());

        // Assert - Verify properties
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("order_67890", properties.GetProperty("order_id").GetString());
        Assert.Equal("USD", properties.GetProperty("currency").GetString());
        Assert.Equal("storefront", properties.GetProperty("source").GetString());
        Assert.Equal("Customer request", properties.GetProperty("cancel_reason").GetString());

        // Assert - Verify products array
        var products = properties.GetProperty("products");
        Assert.Equal(JsonValueKind.Array, products.ValueKind);
        Assert.Single(products.EnumerateArray());
        Assert.Equal("632910392", products[0].GetProperty("product_id").GetString());

        // Assert - Verify metadata
        var metadata = properties.GetProperty("metadata");
        Assert.Equal("https://example.com/orders/67890/cancel", metadata.GetProperty("cancellation_url").GetString());
    }

    [Fact]
    public void OrderCancelledEvent_SerializesCorrectly_WithMinimalProperties()
    {
        // Arrange - Test with only required fields
        var orderCancelled = new OrderCancelledEvent
        {
            ExternalId = "user_123",
            Time = DateTimeOffset.Parse("2024-01-15T10:00:00Z"),
            Properties = new OrderCancelledProperties
            {
                OrderId = "order_minimal",
                Currency = "GBP",
                Source = "web"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(orderCancelled, DefaultJsonSerializerOptions.Options);
        var jsonDoc = JsonDocument.Parse(json);
        var root = jsonDoc.RootElement;

        // Assert - Verify required fields
        Assert.Equal("ecommerce.order_cancelled", root.GetProperty("name").GetString());
        Assert.True(root.TryGetProperty("properties", out var properties));
        Assert.Equal("order_minimal", properties.GetProperty("order_id").GetString());
        Assert.Equal("GBP", properties.GetProperty("currency").GetString());
        Assert.Equal("web", properties.GetProperty("source").GetString());

        // Assert - Verify optional fields are omitted
        Assert.False(properties.TryGetProperty("cancel_reason", out _));
        Assert.False(properties.TryGetProperty("products", out _));
        Assert.False(properties.TryGetProperty("metadata", out _));
    }
}
