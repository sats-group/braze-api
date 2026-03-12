using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Braze.Api.Messages.Send;
using Xunit;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Integration tests for the MessagesSendClient.
/// </summary>
public class MessagesSendClientIntegrationTests
{
    [Fact]
    public async Task TriggerCampaign_SendsCorrectHttpRequest()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123"
        };

        // Act
        await client.TriggerCampaign(request, default);

        // Assert
        Assert.NotNull(handler.LastRequest);
        handler.LastRequest.AssertMethod(HttpMethod.Post);
        handler.LastRequest.AssertUri("/campaigns/trigger/send");
        handler.LastRequest.AssertBearerToken("test-api-key");
        handler.LastRequest.AssertContentType("application/json");
    }

    [Fact]
    public async Task TriggerCampaign_SerializesRequestBodyCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123",
        };

        // Act
        await client.TriggerCampaign(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonProperty(root, "campaign_id", "campaign-123");
    }

    [Fact]
    public async Task TriggerCampaign_WithRecipients_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123",
            Recipients =
            [
                new Recipient() { ExternalUserId = "user-123" },
                new Recipient() { ExternalUserId = "user-456" }
            ]
        };

        // Act
        await client.TriggerCampaign(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "recipients");
        var recipients = root.GetProperty("recipients");
        Assert.Equal(JsonValueKind.Array, recipients.ValueKind);
        Assert.Equal(2, recipients.GetArrayLength());
        HttpRequestAssertions.AssertJsonProperty(recipients[0], "external_user_id", "user-123");
        HttpRequestAssertions.AssertJsonProperty(recipients[1], "external_user_id", "user-456");
    }

    [Fact]
    public async Task TriggerCampaign_WithTriggerProperties_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123",
            TriggerProperties = new Dictionary<string, Property>
            {
                { "property1", Property.Create("value1") },
                { "property2", Property.Create(42) }
            }
        };

        // Act
        await client.TriggerCampaign(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "trigger_properties");
        var props = root.GetProperty("trigger_properties");
        HttpRequestAssertions.AssertJsonPropertyExists(props, "property1");
        HttpRequestAssertions.AssertJsonPropertyExists(props, "property2");
    }

    [Fact]
    public async Task TriggerCampaign_WithAttachments_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123",
            Attachments = new List<Attachment>
            {
                new Attachment
                {
                    FileName = "test-document.pdf",
                    Url = new Uri("https://example.com/test-document.pdf")
                }
            }
        };

        // Act
        await client.TriggerCampaign(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "attachments");
        var attachments = root.GetProperty("attachments");
        Assert.Equal(JsonValueKind.Array, attachments.ValueKind);
        Assert.Equal(1, attachments.GetArrayLength());
        HttpRequestAssertions.AssertJsonProperty(attachments[0], "file_name", "test-document.pdf");
        HttpRequestAssertions.AssertJsonProperty(attachments[0], "url", "https://example.com/test-document.pdf");
    }

    [Fact]
    public async Task TriggerCampaign_SuccessResponse_ReturnsDispatchId()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(
            @"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}",
            rateLimit: 10000,
            rateLimitRemaining: 9999,
            rateLimitReset: 60);

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123"
        };

        // Act
        var response = await client.TriggerCampaign(request, default);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Equal("dispatch-123", response.Value.Id);
        Assert.Equal(10000, response.RateLimitingLimit);
        Assert.Equal(9999, response.RateLimitingRemaining);
        Assert.Equal(60, response.RateLimitingReset);
    }

    [Fact]
    public async Task TriggerCampaign_NullPropertiesNotSerialized()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-123""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123"
            // Other properties are null
        };

        // Act
        await client.TriggerCampaign(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "campaign_id");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "campaign_variation_id");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "recipients");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "trigger_properties");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "broadcast");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "attachments");
    }

    [Fact]
    public async Task TriggerCampaign_InvalidAttachmentsFileName_HandlesErrorResponse()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureErrorResponse(
            @"{""message"": ""The file_name field for email attachments must not contain an extension. We will supply the correct extension based on the content-type of the url.""}");

        var request = new TriggeredCampaign
        {
            CampaignId = "campaign-123",
            Attachments = [new Attachment()
            {
                FileName = "TestFile.pdf",
                Url = new Uri("https://example.com/pdf-file.pdf"),
            }]
            // Other properties are null
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrazeApiException>(() => client.TriggerCampaign(request, default));
        Assert.Contains("The file_name field for email attachments must not contain an extension.", exception.Message);
        Assert.Contains("campaigns/trigger/send", exception.Endpoint?.ToString());
    }

    [Fact]
    public async Task TriggerCanvas_SendsCorrectHttpRequest()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-456""}");

        var request = new TriggeredCanvas
        {
            CanvasId = "canvas-123"
        };

        // Act
        await client.TriggerCanvas(request, default);

        // Assert
        Assert.NotNull(handler.LastRequest);
        handler.LastRequest.AssertMethod(HttpMethod.Post);
        handler.LastRequest.AssertUri("/canvas/trigger/send");
        handler.LastRequest.AssertBearerToken("test-api-key");
        handler.LastRequest.AssertContentType("application/json");
    }

    [Fact]
    public async Task TriggerCanvas_SerializesRequestBodyCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-456""}");

        var request = new TriggeredCanvas
        {
            CanvasId = "canvas-123",
            Broadcast = true
        };

        // Act
        await client.TriggerCanvas(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonProperty(root, "canvas_id", "canvas-123");
        HttpRequestAssertions.AssertJsonProperty(root, "broadcast", true);
    }

    [Fact]
    public async Task TriggerCanvas_WithRecipients_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-456""}");

        var request = new TriggeredCanvas
        {
            CanvasId = "canvas-123",
            Recipients =
            [
                new Recipient { ExternalUserId = "user-123" },
                new Recipient { ExternalUserId = "user-456" },
                new Recipient
                {
                    Email = "yalla@example.com",
                    Prioritization =
                    [
                        Prioritization.Unidentified,
                        Prioritization.MostRecentlyUpdated,
                    ]
                },
            ]
        };

        // Act
        await client.TriggerCanvas(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "recipients");
        var recipients = root.GetProperty("recipients");
        Assert.Equal(JsonValueKind.Array, recipients.ValueKind);
        Assert.Equal(3, recipients.GetArrayLength());
        HttpRequestAssertions.AssertJsonProperty(recipients[0], "external_user_id", "user-123");
        HttpRequestAssertions.AssertJsonProperty(recipients[1], "external_user_id", "user-456");
        HttpRequestAssertions.AssertJsonProperty(recipients[2], "email", "yalla@example.com");
        HttpRequestAssertions.AssertJsonProperty(
            recipients[2],
            "prioritization",
            JsonDocument
                .Parse(
                    """
                    [
                      "unidentified",
                      "most_recently_updated"
                    ]
                    """)
                .RootElement);
    }

    [Fact]
    public async Task TriggerCanvas_WithContext_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-456""}");

        var request = new TriggeredCanvas
        {
            CanvasId = "canvas-123",
            Context = new Dictionary<string, Property>
            {
                { "property1", Property.Create("value1") },
                { "property2", Property.Create(42) }
            }
        };

        // Act
        await client.TriggerCanvas(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "context");
        var props = root.GetProperty("context");
        HttpRequestAssertions.AssertJsonPropertyExists(props, "property1");
        HttpRequestAssertions.AssertJsonPropertyExists(props, "property2");
    }

    [Fact]
    public async Task TriggerCanvas_SuccessResponse_ReturnsDispatchId()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(
            @"{""message"": ""success"", ""dispatch_id"": ""dispatch-456""}",
            rateLimit: 10000,
            rateLimitRemaining: 9999,
            rateLimitReset: 60);

        var request = new TriggeredCanvas
        {
            CanvasId = "canvas-123"
        };

        // Act
        var response = await client.TriggerCanvas(request, default);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Equal("dispatch-456", response.Value.Id);
        Assert.Equal(10000, response.RateLimitingLimit);
        Assert.Equal(9999, response.RateLimitingRemaining);
        Assert.Equal(60, response.RateLimitingReset);
    }

    [Fact]
    public async Task TriggerCanvas_NullPropertiesNotSerialized()
    {
        // Arrange
        var (client, handler) = TestClientFactory.CreateMessagesSendClient();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""dispatch_id"": ""dispatch-456""}");

        var request = new TriggeredCanvas
        {
            CanvasId = "canvas-123"
            // Other properties are null
        };

        // Act
        await client.TriggerCanvas(request, default);

        // Assert
        var body = await handler.LastRequest!.ReadBodyAsJson();
        var root = body.RootElement;

        HttpRequestAssertions.AssertJsonPropertyExists(root, "canvas_id");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "recipients");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "context");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "broadcast");
        HttpRequestAssertions.AssertJsonPropertyDoesNotExist(root, "audience");
    }
}
