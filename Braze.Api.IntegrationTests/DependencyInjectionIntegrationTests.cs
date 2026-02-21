using System.Collections.Generic;
using System.Threading.Tasks;
using Braze.Api.Messages.Send;
using Braze.Api.SubscriptionGroups;
using Braze.Api.UserData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Braze.Api.IntegrationTests;

/// <summary>
/// Integration tests for dependency injection registration and configuration.
/// </summary>
public class DependencyInjectionIntegrationTests
{
    #region Non-Keyed Service Registration

    [Fact]
    public void AddBrazeApi_RegistersAllClients()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddSingleton(configuration);

        // Act
        services.AddBrazeApi();

        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<IUserDataClient>());
        Assert.NotNull(provider.GetService<IMessagesSendClient>());
        Assert.NotNull(provider.GetService<ISubscriptionGroupsClient>());
        Assert.NotNull(provider.GetService<IBrazeProviderFactory>());
    }

    [Fact]
    public void AddBrazeApi_ConfiguresHttpClientCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddSingleton(configuration);

        // Act
        var (_, handler) = TestClientFactory.CreateServiceCollection();
        services.AddBrazeApi();
        services.AddHttpClient("BrazeHttpClient")
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();

        // Assert - Verify options are configured
        var options = provider.GetRequiredService<IOptions<BrazeOptions>>();
        Assert.NotNull(options.Value);
        Assert.Equal("https://rest.iad-01.braze.com/", options.Value.BaseAddress.ToString());
        Assert.Equal("test-api-key", options.Value.ApiKey);
    }

    [Fact]
    public async Task AddBrazeApi_ClientsUseConfiguredHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddSingleton(configuration);

        var handler = new MockHttpMessageHandler();
        handler.ConfigureSuccessResponse(@"{""message"": ""success"", ""attributes_processed"": 1}");

        services.AddBrazeApi();
        services.AddHttpClient("BrazeHttpClient")
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();

        // Act
        var client = provider.GetRequiredService<IUserDataClient>();
        await client.Track(new TrackRequest
        {
            Attributes = [new UserAttribute { ExternalId = "user123" }]
        });

        // Assert - Verify the mock handler was used
        Assert.NotNull(handler.LastRequest);
        handler.LastRequest.AssertMethod(System.Net.Http.HttpMethod.Post);
    }

    #endregion

    #region Keyed Service Registration

    [Fact]
    public void AddBrazeApi_WithKey_RegistersKeyedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("BrazeProduction");
        services.AddSingleton(configuration);

        // Act
        services.AddBrazeApi("production", "BrazeProduction", "production");

        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetKeyedService<IUserDataClient>("production"));
        Assert.NotNull(provider.GetKeyedService<IMessagesSendClient>("production"));
        Assert.NotNull(provider.GetKeyedService<ISubscriptionGroupsClient>("production"));
    }

    [Fact]
    public void AddBrazeApi_MultipleKeys_RegistersMultipleEnvironments()
    {
        // Arrange
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "BrazeProduction:BaseAddress", "https://rest.iad-01.braze.com" },
                { "BrazeProduction:ApiKey", "prod-api-key" },
                { "BrazeStaging:BaseAddress", "https://rest.iad-02.braze.com" },
                { "BrazeStaging:ApiKey", "staging-api-key" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddBrazeApi("production", "BrazeProduction", "production");
        services.AddBrazeApi("staging", "BrazeStaging", "staging");

        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetKeyedService<IUserDataClient>("production"));
        Assert.NotNull(provider.GetKeyedService<IUserDataClient>("staging"));
    }

    #endregion

    #region Provider Factory

    [Fact]
    public void BrazeProviderFactory_CreatesProviderWithAllClients()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        services.AddSingleton(configuration);
        services.AddBrazeApi();

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IBrazeProviderFactory>();

        // Act
        var brazeProvider = factory.Create();

        // Assert
        Assert.NotNull(brazeProvider.UserDataClient);
        Assert.NotNull(brazeProvider.MessagesSendClient);
        Assert.NotNull(brazeProvider.SubscriptionGroupsClient);
    }

    [Fact]
    public void BrazeProviderFactory_WithKey_CreatesProviderWithKeyedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("BrazeProduction");
        services.AddSingleton(configuration);
        services.AddBrazeApi("production", "BrazeProduction");

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IBrazeProviderFactory>();

        // Act
        var brazeProvider = factory.Create("production");

        // Assert
        Assert.NotNull(brazeProvider.UserDataClient);
        Assert.NotNull(brazeProvider.MessagesSendClient);
        Assert.NotNull(brazeProvider.SubscriptionGroupsClient);
    }

    #endregion

    #region Configuration Validation

    [Fact]
    public void BrazeOptions_WithMissingBaseAddress_FailsValidation()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Missing BaseAddress
                { "Braze:ApiKey", "test-api-key" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddBrazeApi();

        var provider = services.BuildServiceProvider();

        // Act & Assert - Options validation happens when options are accessed
        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<BrazeOptions>>().Value);
    }

    [Fact]
    public void BrazeOptions_WithMissingApiKey_FailsValidation()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Braze:BaseAddress", "https://rest.iad-01.braze.com" }
                // Missing ApiKey
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddBrazeApi();

        var provider = services.BuildServiceProvider();

        // Act & Assert - Options validation happens when options are accessed
        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<BrazeOptions>>().Value);
    }

    #endregion

    #region Helper Methods

    private static IConfiguration CreateConfiguration(string sectionName = "Braze")
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"{sectionName}:BaseAddress", "https://rest.iad-01.braze.com" },
                { $"{sectionName}:ApiKey", "test-api-key" }
            })
            .Build();
    }

    #endregion
}
