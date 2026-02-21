using System;
using System.Net.Http;
using Braze.Api.Messages.Send;
using Braze.Api.SubscriptionGroups;
using Braze.Api.UserData;
using Microsoft.Extensions.DependencyInjection;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Helper class for creating test instances of Braze clients with mock HTTP handlers.
/// </summary>
internal static class TestClientFactory
{
    /// <summary>
    /// Create a UserDataClient with a mock HTTP handler.
    /// </summary>
    public static (IUserDataClient client, MockHttpMessageHandler handler) CreateUserDataClient(
        string baseUrl = "https://rest.iad-01.braze.com",
        string apiKey = "test-api-key")
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var client = new UserDataClient(httpClient);
        return (client, handler);
    }

    /// <summary>
    /// Create a SubscriptionGroupsClient with a mock HTTP handler.
    /// </summary>
    public static (ISubscriptionGroupsClient client, MockHttpMessageHandler handler) CreateSubscriptionGroupsClient(
        string baseUrl = "https://rest.iad-01.braze.com",
        string apiKey = "test-api-key")
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var client = new SubscriptionGroupsClient(httpClient);
        return (client, handler);
    }

    /// <summary>
    /// Create a MessagesSendClient with a mock HTTP handler.
    /// </summary>
    public static (IMessagesSendClient client, MockHttpMessageHandler handler) CreateMessagesSendClient(
        string baseUrl = "https://rest.iad-01.braze.com",
        string apiKey = "test-api-key")
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var client = new MessagesSendClient(httpClient);
        return (client, handler);
    }

    /// <summary>
    /// Create a service collection with Braze API configured using a mock handler.
    /// </summary>
    public static (IServiceCollection services, MockHttpMessageHandler handler) CreateServiceCollection(
        string baseUrl = "https://rest.iad-01.braze.com",
        string apiKey = "test-api-key",
        string httpClientName = "BrazeHttpClient")
    {
        var services = new ServiceCollection();
        var handler = new MockHttpMessageHandler();

        // Note: This is just for creating a service collection with a handler.
        // The actual BrazeOptions configuration is handled separately in tests.

        return (services, handler);
    }
}
