using System.Net.Http;
using System.Net.Http.Headers;
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
    /// <param name="optionsKey">The key of the config section to bind the <see cref="BrazeOptions"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBrazeApi(this IServiceCollection services, object key, string optionsKey)
    {
        services
            .AddOptions<BrazeOptions>(optionsKey)
            .BindConfiguration(optionsKey)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient(optionsKey, (provider, client) =>
        {
            var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<BrazeOptions>>();
            var options = optionsMonitor.Get(optionsKey);
            client.BaseAddress = options.BaseAddress;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.ApiKey);
        });

        services.AddKeyedTransient<IUserDataClient, UserDataClient>(
            key,
            (provider, _) => new UserDataClient(provider
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(optionsKey)));

        services.AddBrazeProviderFactory();

        return services;
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
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBrazeApi(this IServiceCollection services, string configSection = "Braze")
    {
        services
            .AddOptions<BrazeOptions>()
            .BindConfiguration(configSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IUserDataClient, UserDataClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<BrazeOptions>>();
            client.BaseAddress = options.Value.BaseAddress;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.Value.ApiKey);
        });

        services.AddBrazeProviderFactory();

        return services;
    }

    private static IServiceCollection AddBrazeProviderFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IBrazeProviderFactory, BrazeProviderFactory>();
        return services;
    }
}
