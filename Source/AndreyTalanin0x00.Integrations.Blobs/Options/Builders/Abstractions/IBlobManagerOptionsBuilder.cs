using System;
using System.Diagnostics.CodeAnalysis;

using AndreyTalanin0x00.Extensions.DependencyInjection;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;

public interface IBlobManagerOptionsBuilder
{
    public IServiceCollection Services { get; }

    public IBlobManagerOptionsBuilder AddBlobStorageProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBlobStorageProvider>(Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProvider>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProvider : class, IBlobStorageProvider;

    public IBlobManagerOptionsBuilder AddBlobStorageProvider<TBlobStorageProvider>(ServiceImplementationFactory<TBlobStorageProvider> blobStorageProviderImplementationFactory, Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProvider>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProvider : class, IBlobStorageProvider;

    public IBlobManagerOptionsBuilder AddBlobStorageProvider<TBlobStorageProviderService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBlobStorageProviderImplementation>(Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProviderImplementation>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService;

    public IBlobManagerOptionsBuilder AddBlobStorageProvider<TBlobStorageProviderService, TBlobStorageProviderImplementation>(ServiceImplementationFactory<TBlobStorageProviderImplementation> blobStorageProviderImplementationFactory, Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProviderImplementation>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService;
}

internal interface IBlobManagerOptionsBuilderInternal : IBlobManagerOptionsBuilder
{
    public BlobManagerOptions Build();
}
