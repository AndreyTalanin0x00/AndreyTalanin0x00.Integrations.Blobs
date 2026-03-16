using System;
using System.Collections.Generic;

using AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

// Disable the IDE0032 (Use auto property) notification to preserve easily recognizable 'injected services' field pattern.
#pragma warning disable IDE0032

namespace AndreyTalanin0x00.Integrations.Blobs.Options.Builders;

internal class BlobStorageProviderOptionsBuilder<TBlobStorageProvider> : IBlobStorageProviderOptionsBuilderInternal<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
    private readonly IServiceCollection m_services;
    private readonly BlobStorageProviderOptions<TBlobStorageProvider> m_blobStorageProviderOptions;

    public BlobStorageProviderOptionsBuilder(IServiceCollection services, BlobStorageProviderOptions<TBlobStorageProvider> blobStorageProviderOptions)
    {
        m_services = services;
        m_blobStorageProviderOptions = blobStorageProviderOptions;
    }

    /// <inheritdoc />
    public IServiceCollection Services => m_services;

    /// <inheritdoc />
    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AddBlobBucket(string blobBucket)
    {
        m_blobStorageProviderOptions.BlobBuckets.Add(blobBucket);

        return this;
    }

    /// <inheritdoc />
    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AddBlobBuckets(IEnumerable<string> blobBuckets)
    {
        foreach (string blobBucket in blobBuckets)
            AddBlobBucket(blobBucket);

        return this;
    }

    /// <inheritdoc />
    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> ClearBlobBuckets()
    {
        m_blobStorageProviderOptions.BlobBuckets.Clear();

        return this;
    }

    /// <inheritdoc />
    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AllowBlobOperation(BlobOperation blobOperation, bool allowed = true)
    {
        m_blobStorageProviderOptions.AllowedBlobOperations.Add(blobOperation);

        return this;
    }

    /// <inheritdoc />
    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AllowBlobOperations(IEnumerable<BlobOperation> blobOperations, bool allowed = true)
    {
        foreach (BlobOperation blobOperation in blobOperations)
            AllowBlobOperation(blobOperation, allowed);

        return this;
    }

    /// <inheritdoc />
    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> ClearAllowedBlobOperations()
    {
        m_blobStorageProviderOptions.AllowedBlobOperations.Clear();

        return this;
    }

    /// <inheritdoc />
    public BlobStorageProviderOptions<TBlobStorageProvider> Build()
    {
        Assert();

        return m_blobStorageProviderOptions;
    }

    protected virtual void Assert()
    {
        if (m_blobStorageProviderOptions.BlobBuckets.Count == 0)
            throw new InvalidOperationException("The blob storage provider's configuration is invalid: no blob buckets.");
        if (m_blobStorageProviderOptions.AllowedBlobOperations.Count == 0)
            throw new InvalidOperationException("The blob storage provider's configuration is invalid: no blob operation is allowed.");

        return;
    }
}
