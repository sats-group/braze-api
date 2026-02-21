using System.Threading;
using System.Threading.Tasks;

namespace Braze.Api.SubscriptionGroups;

/// <summary>
/// A client for the subscription groups endpoints.
/// </summary>
public interface ISubscriptionGroupsClient
{
    /// <summary>
    /// Updates the subscription status for up to 50 users.
    /// </summary>
    /// <param name="request">The subscription status update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The API response.</returns>
    Task<ApiResponse<SubscriptionStatusSetResponse>> SetSubscriptionStatus(SubscriptionStatusSetRequest request, CancellationToken cancellationToken = default);
}
