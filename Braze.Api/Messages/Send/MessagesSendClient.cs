using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api.Messages.Send;

/// <summary>
/// Client for the Braze messages send endpoints.
/// </summary>
public interface IMessagesSendClient
{
    /// <summary>
    /// Send campaign messages using API-triggered delivery.
    /// https://www.braze.com/docs/api/endpoints/messaging/send_messages/post_send_triggered_campaigns
    /// </summary>
    Task<ApiResponse<DispatchId>> TriggerCampaign(TriggeredCampaign triggeredCampaign, CancellationToken cancellationToken);
}

/// <summary>
/// Dispatch id model.
/// </summary>
public class DispatchId
{
    /// <summary>
    /// The dispatch id for message-sending endpoints.
    /// </summary>
    [JsonPropertyName("dispatch_id")]
    public required string Id { get; init; }
}

internal class MessagesSendClient(HttpClient httpClient) : IMessagesSendClient
{
    public async Task<ApiResponse<DispatchId>> TriggerCampaign(TriggeredCampaign triggeredCampaign, CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("campaigns/trigger/send", UriKind.Relative))
        {
            Content = JsonContent.Create(triggeredCampaign)
        };

        using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

        return await responseMessage.CreateApiResponse<DispatchId>(cancellationToken);
    }
}
