using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using AndreyTalanin0x00.Extensions.DependencyInjection;
using AndreyTalanin0x00.Integrations.Blobs.Options;
using AndreyTalanin0x00.Integrations.Blobs.Options.Builders;
using AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;
using AndreyTalanin0x00.Integrations.Blobs.Services;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

// Disable the IDE0001 (Simplify name) notification to preserve explicit types.
#pragma warning disable IDE0001

namespace AndreyTalanin0x00.Integrations.Blobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobManager(this IServiceCollection services, Action<IBlobManagerOptionsBuilder> configureAction)
    {
        services.AddCustomBlobManagerCore<BlobManager>(configureAction: configureAction);

        return services;
    }

    public static IServiceCollection AddCustomBlobManager<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBlobManager>(this IServiceCollection services, Action<IBlobManagerOptionsBuilder> configureAction)
        where TBlobManager : class, IBlobManager
    {
        services.AddCustomBlobManagerCore<TBlobManager>(configureAction: configureAction);

        return services;
    }

    public static IServiceCollection AddCustomBlobManager<TBlobManager>(this IServiceCollection services, ServiceImplementationFactory<TBlobManager> blobManagerImplementationFactory, Action<IBlobManagerOptionsBuilder> configureAction)
        where TBlobManager : class, IBlobManager
    {
        services.AddCustomBlobManagerCore<TBlobManager>(blobManagerImplementationFactory, configureAction);

        return services;
    }

    public static IServiceCollection AddBlobManagerFactory(this IServiceCollection services)
    {
        services.AddCustomBlobManagerFactoryCore<BlobManagerFactory>();

        return services;
    }

    public static IServiceCollection AddCustomBlobManagerFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBlobManagerFactory>(this IServiceCollection services)
        where TBlobManagerFactory : class, IBlobManagerFactory
    {
        services.AddCustomBlobManagerFactoryCore<TBlobManagerFactory>();

        return services;
    }

    public static IServiceCollection AddCustomBlobManagerFactory<TBlobManagerFactory>(this IServiceCollection services, ServiceImplementationFactory<TBlobManagerFactory> blobManagerFactoryImplementationFactory)
        where TBlobManagerFactory : class, IBlobManagerFactory
    {
        services.AddCustomBlobManagerFactoryCore<TBlobManagerFactory>(blobManagerFactoryImplementationFactory);

        return services;
    }

    private static IServiceCollection AddCustomBlobManagerCore<TBlobManager>(this IServiceCollection services, ServiceImplementationFactory<TBlobManager>? blobManagerImplementationFactory = null, Action<IBlobManagerOptionsBuilder>? configureAction = null)
        where TBlobManager : class, IBlobManager
    {
        BlobManagerOptions blobManagerOptions = new();
        BlobManagerOptionsBuilder blobManagerOptionsBuilder = new(services, blobManagerOptions);

        if (configureAction is not null)
            configureAction(blobManagerOptionsBuilder);

        blobManagerOptions = blobManagerOptionsBuilder.Build();

        ConfigureBlobManagerServices(services, blobManagerOptions);

        if (blobManagerImplementationFactory is not null)
            services.AddTransient<IBlobManager, TBlobManager>(serviceProvider => blobManagerImplementationFactory(serviceProvider));
        else
            services.AddTransient<IBlobManager, TBlobManager>();

        return services;
    }

    private static void ConfigureBlobManagerServices(IServiceCollection services, BlobManagerOptions blobManagerOptions)
    {
        const string blobManagerOptionsClassName =
            nameof(BlobManagerOptions);

        const string addBlobStorageProviderServiceCollectionVisitorsPropertyName =
            nameof(BlobManagerOptions.AddBlobStorageProviderServiceCollectionVisitors);

        const string buildMethodName =
            nameof(IBlobManagerOptionsBuilderInternal.Build);

        if (blobManagerOptions.AddBlobStorageProviderServiceCollectionVisitors.Count == 0)
            throw new UnreachableException($"An {blobManagerOptionsClassName} instance has its {addBlobStorageProviderServiceCollectionVisitorsPropertyName} property empty after the {buildMethodName} method has been called.");

        foreach (Action<IServiceCollection> addBlobStorageProviderServiceCollectionVisitor in blobManagerOptions.AddBlobStorageProviderServiceCollectionVisitors)
            addBlobStorageProviderServiceCollectionVisitor(services);
    }

    private static IServiceCollection AddCustomBlobManagerFactoryCore<TBlobManagerFactory>(this IServiceCollection services, ServiceImplementationFactory<TBlobManagerFactory>? blobManagerFactoryImplementationFactory = null)
        where TBlobManagerFactory : class, IBlobManagerFactory
    {
        ConfigureBlobManagerFactoryServices();

        if (blobManagerFactoryImplementationFactory is not null)
            services.AddTransient<IBlobManagerFactory, TBlobManagerFactory>(serviceProvider => blobManagerFactoryImplementationFactory(serviceProvider));
        else
            services.AddTransient<IBlobManagerFactory, TBlobManagerFactory>();

        return services;
    }

    private static void ConfigureBlobManagerFactoryServices()
    {
    }
}
