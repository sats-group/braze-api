using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api.SubscriptionGroups;

/// <inheritdoc/>
internal class SubscriptionGroupsClient(HttpClient httpClient) : ISubscriptionGroupsClient
{
    /// <inheritdoc/>
    public async Task<ApiResponse<SubscriptionStatusSetResponse>> SetSubscriptionStatus(SubscriptionStatusSetRequest request, CancellationToken cancellationToken = default)
    {
        var requestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("v2/subscription/status/set", UriKind.Relative))
        {
            Content = JsonContent.Create(request)
        };

        using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

        return await responseMessage.CreateApiResponse<SubscriptionStatusSetResponse>(cancellationToken);
    }
}
