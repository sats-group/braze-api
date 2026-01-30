using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Braze.Api.UserData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Braze.Api;

/// <summary>
/// The options for the Braze API.
/// </summary>
public class BrazeOptions
{
    /// <summary>
    /// The base address of the Braze API.
    /// </summary>
    [Required]
    public required Uri BaseAddress { get; init; }

    /// <summary>
    /// The API key for the Braze API.
    /// </summary>
    [Required]
    public required string ApiKey { get; init; }
}

/// <summary>
/// Extension methods for adding Braze.Api services to the DI
/// </summary>
public static class BrazeConfiguration
{
    /// <summary>
    /// Add necessary Braze.Api services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the necessary Braze.Api services.</param>
    /// <param name="optionsKey">The key of the config section to bind the <see cref="BrazeOptions"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBrazeApi(this IServiceCollection services, string optionsKey = "Braze")
    {
        services
            .AddOptions<BrazeOptions>(optionsKey)
            .Configure<IConfiguration>((obj, config) =>
                config.GetSection(optionsKey).Bind(obj))
            .ValidateOnStart();

        services.AddHttpClient<IUserDataClient, UserDataClient>((provider, client) =>
        {
            var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<BrazeOptions>>();
            var options = optionsMonitor.Get(optionsKey);
            client.BaseAddress = options.BaseAddress;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.ApiKey);
        });

        return services;
    }
}
