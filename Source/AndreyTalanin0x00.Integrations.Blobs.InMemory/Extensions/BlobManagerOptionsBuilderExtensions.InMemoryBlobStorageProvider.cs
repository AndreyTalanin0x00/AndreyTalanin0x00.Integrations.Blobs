using System;

using AndreyTalanin0x00.Extensions.DependencyInjection;
using AndreyTalanin0x00.Integrations.Blobs.Configuration;
using AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;
using AndreyTalanin0x00.Integrations.Blobs.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Disable the IDE0001 (Simplify name) notification to preserve explicit types.
#pragma warning disable IDE0001

namespace AndreyTalanin0x00.Integrations.Blobs.Extensions;

public static class InMemoryBlobStorageProviderBlobManagerOptionsBuilderExtensions
{
    public static IBlobManagerOptionsBuilder AddInMemoryBlobStorageProvider(this IBlobManagerOptionsBuilder blobManagerOptionsBuilder, IConfiguration configuration, Action<IBlobStorageProviderOptionsBuilder<InMemoryBlobStorageProvider>>? configureAction = null, bool clearDefaultAllowedOperations = false)
    {
        IServiceCollection services = blobManagerOptionsBuilder.Services;

        services.AddSingleton<InMemoryBlobStorageProvider>();

        InMemoryBlobStorageProvider GetBlobStorageProvider(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<InMemoryBlobStorageProvider>();

        blobManagerOptionsBuilder.AddBlobStorageProvider<InMemoryBlobStorageProvider>(GetBlobStorageProvider, Configure, ServiceLifetime.Singleton, clearDefaultAllowedOperations);

        services.AddHosted<InMemoryBlobStorageProvider>(GetBlobStorageProvider);

        void Configure(IBlobStorageProviderOptionsBuilder<InMemoryBlobStorageProvider> blobStorageProviderOptionsBuilder)
        {
            if (configureAction is not null)
                configureAction(blobStorageProviderOptionsBuilder);

            return;
        }

        IConfigurationSection inMemoryBlobStorageProviderConfigurationSection = configuration
            .GetSection(BlobStorageProvidersConfiguration.SectionName)
            .GetSection(InMemoryBlobStorageProviderConfiguration.SectionName);

        services.Configure<InMemoryBlobStorageProviderConfiguration>(inMemoryBlobStorageProviderConfigurationSection);

        return blobManagerOptionsBuilder;
    }
}
