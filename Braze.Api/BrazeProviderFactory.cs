using System;
using System.ComponentModel.DataAnnotations;
using Braze.Api.Messages.Send;
using Braze.Api.UserData;
using Microsoft.Extensions.DependencyInjection;

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
/// A provider class that can be used to get different kinds braze clients.
/// </summary>
public interface IBrazeProvider
{
    /// <summary>
    /// Gets the <see cref="IUserDataClient"/> from this provider.
    /// </summary>
    IUserDataClient UserDataClient { get; }

    /// <summary>
    /// Gets the <see cref="IMessagesSendClient"/> from this provider.
    /// </summary>
    IMessagesSendClient MessagesSendClient { get; }
}

internal class BrazeProvider(IServiceProvider provider, object? key) : IBrazeProvider
{
    private readonly Lazy<IUserDataClient> _userDataClient = new(() =>
        key is not null
            ? provider.GetRequiredKeyedService<IUserDataClient>(key)
            : provider.GetRequiredService<IUserDataClient>());

    private readonly Lazy<IMessagesSendClient> _messagesSendClient = new(() =>
        key is not null
            ? provider.GetRequiredKeyedService<IMessagesSendClient>(key)
            : provider.GetRequiredService<IMessagesSendClient>());

    public IUserDataClient UserDataClient => _userDataClient.Value;

    public IMessagesSendClient MessagesSendClient => _messagesSendClient.Value;
}

/// <summary>
/// A factory class for <see cref="IBrazeProvider"/>.
/// </summary>
public interface IBrazeProviderFactory
{
    /// <summary>
    /// Create a <see cref="IBrazeProvider"/>.
    /// </summary>
    /// <param name="key">If provided this will create a <see cref="IBrazeProvider"/> that gets Brazed clients as keyed services.</param>
    /// <returns>A <see cref="IBrazeProvider"/>.</returns>
    IBrazeProvider Create(object? key = null);
}

internal class BrazeProviderFactory(IServiceProvider provider) : IBrazeProviderFactory
{
    public IBrazeProvider Create(object? key = null) => new BrazeProvider(provider, key);
}
