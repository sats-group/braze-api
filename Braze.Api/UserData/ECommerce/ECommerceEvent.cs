using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Braze.Api.UserData.ECommerce;

/// <summary>
/// Strongly typed support for Braze's implementation of ecommerce events
/// </summary>
public class ECommerceEvent<T> : Event where T : ECommerceProperty
{
    /// <summary>
    /// The properties for this ecommerce event.
    /// </summary>
    [JsonPropertyName("properties")]
    public required T Properties { get; init; }
}

/// <summary>
/// Event triggered when a customer views a product detail page.
/// </summary>
public class ProductViewedEvent : ECommerceEvent<ProductViewedProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductViewedEvent"/> class with the event name set to "ecommerce.product_viewed".
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor
    public ProductViewedEvent()
#pragma warning restore CS8618
    {
        Name = "ecommerce.product_viewed";
    }
}

/// <summary>
/// Event triggered when a customer adds, removes, or updates products in their cart.
/// </summary>
public class CartUpdatedEvent : ECommerceEvent<CartUpdatedProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CartUpdatedEvent"/> class with the event name set to "ecommerce.cart_updated".
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor
    public CartUpdatedEvent()
#pragma warning restore CS8618
    {
        Name = "ecommerce.cart_updated";
    }
}

/// <summary>
/// Event triggered when a customer begins the checkout process.
/// </summary>
public class CheckoutStartedEvent : ECommerceEvent<CheckoutStartedProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutStartedEvent"/> class with the event name set to "ecommerce.checkout_started".
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor
    public CheckoutStartedEvent()
#pragma warning restore CS8618
    {
        Name = "ecommerce.checkout_started";
    }
}

/// <summary>
/// Event triggered when an order is successfully placed.
/// </summary>
public class OrderPlacedEvent : ECommerceEvent<OrderPlacedProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderPlacedEvent"/> class with the event name set to "ecommerce.order_placed".
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor
    public OrderPlacedEvent()
#pragma warning restore CS8618
    {
        Name = "ecommerce.order_placed";
    }
}

/// <summary>
/// Event triggered when an order is refunded.
/// </summary>
public class OrderRefundedEvent : ECommerceEvent<OrderRefundedProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRefundedEvent"/> class with the event name set to "ecommerce.order_refunded".
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor
    public OrderRefundedEvent()
#pragma warning restore CS8618
    {
        Name = "ecommerce.order_refunded";
    }
}

/// <summary>
/// Event triggered when an order is cancelled.
/// </summary>
public class OrderCancelledEvent : ECommerceEvent<OrderCancelledProperties>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderCancelledEvent"/> class with the event name set to "ecommerce.order_cancelled".
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable property must contain a non-null value when exiting constructor
    public OrderCancelledEvent()
#pragma warning restore CS8618
    {
        Name = "ecommerce.order_cancelled";
    }
}

/// <summary>
/// Base class for ecommerce event properties.
/// </summary>
public abstract class ECommerceProperty
{
}

/// <summary>
/// A discount applied to an order.
/// </summary>
public class Discount
{
    /// <summary>
    /// Discount code (campaigns, coupons, etc.)
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    /// <summary>
    /// The amount of the discount.
    /// </summary>
    [JsonPropertyName("amount")]
    public required decimal Amount { get; init; }
}

/// <summary>
/// A product in the context of an order
/// </summary>
public class Product
{
    /// <summary>
    /// A unique identifier for the product that was viewed. This value be can be the product ID or SKU.
    /// </summary>
    [JsonPropertyName("product_id")]
    public required string ProductId { get; init; }

    /// <summary>
    /// The name of the product that was viewed.
    /// </summary>
    [JsonPropertyName("product_name")]
    public required string ProductName { get; init; }

    /// <summary>
    /// A unique identifier for the product variant. An example is shirt_medium_blue
    /// </summary>
    [JsonPropertyName("variant_id")]
    public string? VariantId { get; init; }

    /// <summary>
    /// Number of units of the product in the cart.
    /// </summary>
    [JsonPropertyName("quantity")]
    public required int Quantity { get; init; }

    /// <summary>
    /// The variant unit price of the product at the time of viewing.
    /// </summary>
    [JsonPropertyName("price")]
    public required decimal Price { get; init; }

    /// <summary>
    /// The URL of the product image, if available.
    /// </summary>
    [JsonPropertyName("image_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ImageUrl { get; init; }

    /// <summary>
    /// The URL of the product page for more details.
    /// </summary>
    [JsonPropertyName("product_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ProductUrl { get; init; }

    /// <summary>
    /// Additional metadata field about the product that the customer wants to add for their use cases. For Shopify, we will add SKU.
    /// </summary>
    /// <remarks>
    /// This will have a limit based on Braze's general event properties limit of 50kb.
    /// </remarks>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Properties for the ecommerce.product_viewed event.
/// </summary>
public class ProductViewedProperties : ECommerceProperty
{
    /// <summary>
    /// A unique identifier for the product. This value can be the product ID or SKU.
    /// </summary>
    [JsonPropertyName("product_id")]
    public required string ProductId { get; init; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    [JsonPropertyName("product_name")]
    public required string ProductName { get; init; }

    /// <summary>
    /// A unique identifier for the product variant. An example is shirt_medium_blue.
    /// </summary>
    [JsonPropertyName("variant_id")]
    public required string VariantId { get; init; }

    /// <summary>
    /// The unit price of the product.
    /// </summary>
    [JsonPropertyName("price")]
    public required decimal Price { get; init; }

    /// <summary>
    /// Currency in which the product is priced.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// Source the event is derived from. (For Shopify, this is "storefront").
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// The URL of the product image, if available.
    /// </summary>
    [JsonPropertyName("image_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ImageUrl { get; init; }

    /// <summary>
    /// The URL of the product page for more details.
    /// </summary>
    [JsonPropertyName("product_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? ProductUrl { get; init; }

    /// <summary>
    /// Additional metadata about the product view.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Properties for the ecommerce.cart_updated event.
/// </summary>
public class CartUpdatedProperties : ECommerceProperty
{
    /// <summary>
    /// Unique identifier for the cart. If you are not using a third-party platform that provides a cart_id, you can use the Braze session ID.
    /// </summary>
    [JsonPropertyName("cart_id")]
    public required string CartId { get; init; }

    /// <summary>
    /// Total monetary value of the cart.
    /// </summary>
    [JsonPropertyName("total_value")]
    public required decimal TotalValue { get; init; }

    /// <summary>
    /// Currency in which the cart is valued.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// List of products currently in the cart.
    /// </summary>
    [JsonPropertyName("products")]
    public required IEnumerable<Product> Products { get; init; }

    /// <summary>
    /// Source the event is derived from. (For Shopify, this is "storefront").
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// Additional metadata about the cart update.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Properties for the ecommerce.checkout_started event.
/// </summary>
public class CheckoutStartedProperties : ECommerceProperty
{
    /// <summary>
    /// Unique identifier for the checkout session.
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public required string CheckoutId { get; init; }

    /// <summary>
    /// If you are not using a third-party platform that provides a cart_id, you can use the Braze session ID.
    /// </summary>
    [JsonPropertyName("cart_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CartId { get; init; }

    /// <summary>
    /// Total monetary value of the checkout.
    /// </summary>
    [JsonPropertyName("total_value")]
    public required decimal TotalValue { get; init; }

    /// <summary>
    /// Currency in which the checkout is valued.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// List of products in the checkout.
    /// </summary>
    [JsonPropertyName("products")]
    public required IEnumerable<Product> Products { get; init; }

    /// <summary>
    /// Source the event is derived from. (For Shopify, this is "storefront").
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// Additional metadata about the checkout.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Properties for the ecommerce.order_placed event.
/// </summary>
public class OrderPlacedProperties : ECommerceProperty
{
    /// <summary>
    /// Unique identifier for the order placed.
    /// </summary>
    [JsonPropertyName("order_id")]
    public required string OrderId { get; init; }

    /// <summary>
    /// If you are not using a third-party platform that provides a cart_id, you can use the Braze session ID.
    /// </summary>
    [JsonPropertyName("cart_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CartId { get; init; }

    /// <summary>
    /// Total monetary value of the cart.
    /// </summary>
    [JsonPropertyName("total_value")]
    public required decimal TotalValue { get; init; }

    /// <summary>
    /// Currency in which the cart is valued.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// Total discount amount applied to the order.
    /// </summary>
    [JsonPropertyName("total_discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalDiscounts { get; init; }

    /// <summary>
    /// Detailed list of discounts applied to the order.
    /// </summary>
    [JsonPropertyName("discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Discount>? Discounts { get; init; }

    /// <summary>
    /// List of products in the order.
    /// </summary>
    [JsonPropertyName("products")]
    public required IEnumerable<Product> Products { get; init; }

    /// <summary>
    /// Source the event is derived from. (For Shopify, this is "storefront").
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// Additional metadata about the order.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Properties for the ecommerce.order_refunded event.
/// </summary>
public class OrderRefundedProperties : ECommerceProperty
{
    /// <summary>
    /// Unique identifier for the order that was refunded.
    /// </summary>
    [JsonPropertyName("order_id")]
    public required string OrderId { get; init; }

    /// <summary>
    /// Total monetary value of the order.
    /// </summary>
    [JsonPropertyName("total_value")]
    public required decimal TotalValue { get; init; }

    /// <summary>
    /// Currency in which the order is valued.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// Total discount amount applied to the order.
    /// </summary>
    [JsonPropertyName("total_discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalDiscounts { get; init; }

    /// <summary>
    /// Detailed list of discounts applied to the order.
    /// </summary>
    [JsonPropertyName("discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Discount>? Discounts { get; init; }

    /// <summary>
    /// List of products included in the refund (required per Braze API specification).
    /// </summary>
    [JsonPropertyName("products")]
    public required IEnumerable<Product> Products { get; init; }

    /// <summary>
    /// Source the event is derived from. (For Shopify, this is "storefront").
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// Additional metadata about the refund.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Properties for the ecommerce.order_cancelled event.
/// </summary>
public class OrderCancelledProperties : ECommerceProperty
{
    /// <summary>
    /// Unique identifier for the order that was cancelled.
    /// </summary>
    [JsonPropertyName("order_id")]
    public required string OrderId { get; init; }

    /// <summary>
    /// Reason why the order was cancelled.
    /// </summary>
    [JsonPropertyName("cancel_reason")]
    public required string CancelReason { get; init; }

    /// <summary>
    /// Total monetary value of the order.
    /// </summary>
    [JsonPropertyName("total_value")]
    public required decimal TotalValue { get; init; }

    /// <summary>
    /// Currency in which the order was valued.
    /// </summary>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// Total discount amount applied to the order.
    /// </summary>
    [JsonPropertyName("total_discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TotalDiscounts { get; init; }

    /// <summary>
    /// Detailed list of discounts applied to the order.
    /// </summary>
    [JsonPropertyName("discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<Discount>? Discounts { get; init; }

    /// <summary>
    /// List of products included in the cancelled order (required per Braze API specification).
    /// </summary>
    [JsonPropertyName("products")]
    public required IEnumerable<Product> Products { get; init; }

    /// <summary>
    /// Source the event is derived from. (For Shopify, this is "storefront").
    /// </summary>
    [JsonPropertyName("source")]
    public required string Source { get; init; }

    /// <summary>
    /// Additional metadata about the cancellation.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Metadata { get; init; }
}
