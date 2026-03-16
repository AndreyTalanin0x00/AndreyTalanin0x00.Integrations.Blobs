using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AndreyTalanin0x00.Extensions.DependencyInjection;
using AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// Disable the IDE0032 (Use auto property) notification to preserve easily recognizable 'injected services' field pattern.
#pragma warning disable IDE0032

namespace AndreyTalanin0x00.Integrations.Blobs.Options.Builders;

internal class BlobManagerOptionsBuilder : IBlobManagerOptionsBuilderInternal
{
    private readonly IServiceCollection m_services;
    private readonly BlobManagerOptions m_blobManagerOptions;

    public BlobManagerOptionsBuilder(IServiceCollection services, BlobManagerOptions blobManagerOptions)
    {
        m_services = services;
        m_blobManagerOptions = blobManagerOptions;
    }

    public IServiceCollection Services => m_services;

    /// <inheritdoc />
    public IBlobManagerOptionsBuilder AddBlobStorageProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBlobStorageProvider>(Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProvider>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        ServiceImplementationFactory<TBlobStorageProvider>? blobStorageProviderImplementationFactory = null;

        AddBlobStorageProviderCore<IBlobStorageProvider, TBlobStorageProvider>(blobStorageProviderImplementationFactory, configureAction, serviceLifetime, emptyDefaultAllowedOperations);

        return this;
    }

    /// <inheritdoc />
    public IBlobManagerOptionsBuilder AddBlobStorageProvider<TBlobStorageProvider>(ServiceImplementationFactory<TBlobStorageProvider> blobStorageProviderImplementationFactory, Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProvider>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        AddBlobStorageProviderCore<IBlobStorageProvider, TBlobStorageProvider>(blobStorageProviderImplementationFactory, configureAction, serviceLifetime, emptyDefaultAllowedOperations);

        return this;
    }

    /// <inheritdoc />
    public IBlobManagerOptionsBuilder AddBlobStorageProvider<TBlobStorageProviderService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBlobStorageProviderImplementation>(Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProviderImplementation>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService
    {
        ServiceImplementationFactory<TBlobStorageProviderImplementation>? blobStorageProviderImplementationFactory = null;

        AddBlobStorageProviderCore<TBlobStorageProviderService, TBlobStorageProviderImplementation>(blobStorageProviderImplementationFactory, configureAction, serviceLifetime, emptyDefaultAllowedOperations);

        return this;
    }

    /// <inheritdoc />
    public IBlobManagerOptionsBuilder AddBlobStorageProvider<TBlobStorageProviderService, TBlobStorageProviderImplementation>(ServiceImplementationFactory<TBlobStorageProviderImplementation> blobStorageProviderImplementationFactory, Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProviderImplementation>>? configureAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, bool emptyDefaultAllowedOperations = false)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService
    {
        AddBlobStorageProviderCore<TBlobStorageProviderService, TBlobStorageProviderImplementation>(blobStorageProviderImplementationFactory, configureAction, serviceLifetime, emptyDefaultAllowedOperations);

        return this;
    }

    /// <inheritdoc />
    public BlobManagerOptions Build()
    {
        Assert();

        return m_blobManagerOptions;
    }

    protected virtual void Assert()
    {
        if (m_blobManagerOptions.AddBlobStorageProviderServiceCollectionVisitors.Count == 0)
            throw new InvalidOperationException("The blob manager's configuration is invalid: no blob storage provider is specified.");

        return;
    }

    private BlobManagerOptionsBuilder AddBlobStorageProviderCore<TBlobStorageProviderService, TBlobStorageProviderImplementation>(ServiceImplementationFactory<TBlobStorageProviderImplementation>? blobStorageProviderImplementationFactory, Action<IBlobStorageProviderOptionsBuilder<TBlobStorageProviderImplementation>>? configureAction, ServiceLifetime serviceLifetime, bool emptyDefaultAllowedOperations)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService
    {
        m_blobManagerOptions.AddBlobStorageProviderServiceCollectionVisitors.Add(AddBlobStorageProviderCore);

        void AddBlobStorageProviderCore(IServiceCollection services)
        {
            BlobStorageProviderOptions<TBlobStorageProviderImplementation> blobStorageProviderOptions = new();
            BlobStorageProviderOptionsBuilder<TBlobStorageProviderImplementation> blobStorageProviderOptionsBuilder = new(services, blobStorageProviderOptions);

            ConfigureBlobStorageProviderOptions<TBlobStorageProviderService, TBlobStorageProviderImplementation>(blobStorageProviderOptions, emptyDefaultAllowedOperations);

            if (configureAction is not null)
                configureAction(blobStorageProviderOptionsBuilder);

            ValidateBlobStorageProviderOptions<TBlobStorageProviderService, TBlobStorageProviderImplementation>(blobStorageProviderOptions);

            blobStorageProviderOptions = blobStorageProviderOptionsBuilder.Build();

            OptionsWrapper<BlobStorageProviderOptions<TBlobStorageProviderImplementation>> blobStorageProviderOptionsWrapper = new(blobStorageProviderOptions);

            services.AddSingleton<IOptions<BlobStorageProviderOptions<TBlobStorageProviderImplementation>>>(blobStorageProviderOptionsWrapper);

            Type blobStorageProviderServiceType = typeof(TBlobStorageProviderService);
            Type blobStorageProviderImplementationType = typeof(TBlobStorageProviderImplementation);

            ServiceDescriptor serviceDescriptor = blobStorageProviderImplementationFactory is not null
                ? new ServiceDescriptor(blobStorageProviderServiceType, (serviceProvider) => blobStorageProviderImplementationFactory(serviceProvider), serviceLifetime)
                : new ServiceDescriptor(blobStorageProviderServiceType, blobStorageProviderImplementationType, serviceLifetime);

            services.Add(serviceDescriptor);
        }

        return this;
    }

    private static void ConfigureBlobStorageProviderOptions<TBlobStorageProviderService, TBlobStorageProviderImplementation>(BlobStorageProviderOptions<TBlobStorageProviderImplementation> blobStorageProviderOptions, bool emptyDefaultAllowedOperations)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService
    {
        if (emptyDefaultAllowedOperations)
            return;

        ICollection<BlobOperation> allowedBlobOperations = blobStorageProviderOptions.AllowedBlobOperations;
        IReadOnlyCollection<BlobOperation> defaultSupportedBlobOperations = TBlobStorageProviderImplementation.GetDefaultSupportedBlobOperations();

        foreach (BlobOperation defaultSupportedBlobOperation in defaultSupportedBlobOperations)
            allowedBlobOperations.Add(defaultSupportedBlobOperation);
    }

    private static void ValidateBlobStorageProviderOptions<TBlobStorageProviderService, TBlobStorageProviderImplementation>(BlobStorageProviderOptions<TBlobStorageProviderImplementation> blobStorageProviderOptions)
        where TBlobStorageProviderService : class, IBlobStorageProvider
        where TBlobStorageProviderImplementation : class, TBlobStorageProviderService
    {
        ICollection<BlobOperation> allowedBlobOperations = blobStorageProviderOptions.AllowedBlobOperations;
        IReadOnlyCollection<BlobOperation> defaultSupportedBlobOperations = TBlobStorageProviderImplementation.GetDefaultSupportedBlobOperations();
        HashSet<BlobOperation> defaultSupportedBlobOperationsHashSet = [.. defaultSupportedBlobOperations];

        foreach (BlobOperation allowedBlobOperation in allowedBlobOperations)
        {
            if (!defaultSupportedBlobOperationsHashSet.Contains(allowedBlobOperation))
            {
                string blobStorageProviderTypeName = typeof(TBlobStorageProviderImplementation).Name;

                const string exceptionMessageFormat = "Can not allow the {0} operation for the {1} blob storage provider because it is not supported.";

                string exceptionMessage = string.Format(exceptionMessageFormat, allowedBlobOperation, blobStorageProviderTypeName);

                throw new NotSupportedException(exceptionMessage);
            }
        }
    }
}
