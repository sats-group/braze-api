using System.Net.Http;
using System.Net.Http.Headers;
using Braze.Api.Messages.Send;
using Braze.Api.UserData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Braze.Api;

/// <summary>
/// Extension methods for adding Braze.Api services to the DI
/// </summary>
public static class BrazeConfiguration
{
    /// <summary>
    /// Add necessary Braze.Api services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the necessary Braze.Api services.</param>
    /// <param name="key">The key use to register Braze clients as keyed services.</param>
    /// <param name="configSectionPath">The key of the config section to bind the <see cref="BrazeOptions"/> to.</param>
    /// <param name="httpClientName">Optional logical name of the http client used by the Braze clients, default: <paramref name="configSectionPath"/>.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> for the <see cref="HttpClient"/> used by the Braze clients.</returns>
    public static IHttpClientBuilder AddBrazeApi(
        this IServiceCollection services,
        object key,
        string configSectionPath,
        string? httpClientName = null)
    {
        httpClientName ??= configSectionPath;

        services
            .AddOptions<BrazeOptions>(configSectionPath)
            .BindConfiguration(configSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddKeyedBraze<IUserDataClient, UserDataClient>(key, httpClientName)
            .AddKeyedBraze<IMessagesSendClient, MessagesSendClient>(key, httpClientName)
            .AddBrazeProviderFactory();

        return services.AddHttpClient(
            httpClientName,
            (provider, client) =>
            {
                var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<BrazeOptions>>();
                var options = optionsMonitor.Get(configSectionPath);
                client.BaseAddress = options.BaseAddress;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.ApiKey);
            });
    }

    /// <summary>
    /// Add necessary Braze.Api services.
    /// </summary>
    /// <remarks>
    /// If you need to register multiple environments,
    /// use the overload that also excepts a 'key' so that the clients are registered as keyed services.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the necessary Braze.Api services.</param>
    /// <param name="configSection">Optional config section name, default: "Braze".</param>
    /// <param name="httpClientName">Optional logical name of the http client used by the Braze clients, default: "BrazeHttpClient".</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> for the <see cref="HttpClient"/> used by the Braze clients.</returns>
    public static IHttpClientBuilder AddBrazeApi(
        this IServiceCollection services,
        string configSection = "Braze",
        string httpClientName = "BrazeHttpClient")
    {
        services
            .AddOptions<BrazeOptions>()
            .BindConfiguration(configSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddBrazeClient<IUserDataClient, UserDataClient>(httpClientName)
            .AddBrazeClient<IMessagesSendClient, MessagesSendClient>(httpClientName)
            .AddBrazeProviderFactory();

        return services.AddHttpClient(
            httpClientName,
            static (provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<BrazeOptions>>();
                client.BaseAddress = options.Value.BaseAddress;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
            });
    }

    private static IServiceCollection AddBrazeClient<TInterface, TImplementation>(
        this IServiceCollection services,
        string httpClientName)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddTransient<TInterface, TImplementation>(
            provider =>
            {
                var httpClient = provider
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(httpClientName);
                return ActivatorUtilities.CreateInstance<TImplementation>(provider, httpClient);
            });
        return services;
    }

    private static IServiceCollection AddKeyedBraze<TInterface, TImplementation>(
        this IServiceCollection services,
        object key,
        string httpClientName)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        services.AddKeyedTransient<TInterface, TImplementation>(
            key,
            (provider, _) =>
            {
                var httpClient = provider
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(httpClientName);
                return ActivatorUtilities.CreateInstance<TImplementation>(provider, httpClient);
            });

        return services;
    }

    private static IServiceCollection AddBrazeProviderFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IBrazeProviderFactory, BrazeProviderFactory>();
        return services;
    }
}
