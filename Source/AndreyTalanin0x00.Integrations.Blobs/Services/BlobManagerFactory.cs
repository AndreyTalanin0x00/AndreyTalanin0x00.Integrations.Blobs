using System;
using System.Collections.Generic;
using System.Linq;

using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AndreyTalanin0x00.Integrations.Blobs.Services;

public class BlobManagerFactory : IBlobManagerFactory
{
    private readonly IServiceProvider m_serviceProvider;

    public BlobManagerFactory(IServiceProvider serviceProvider)
    {
        m_serviceProvider = serviceProvider;
    }

    public IBlobManager CreateBlobManager()
    {
        IBlobStorageProvider[] blobStorageProviders = GetBlobStorageProviders<IBlobStorageProvider>();

        BlobManager blobManager = new(blobStorageProviders);

        return blobManager;
    }

    public IBlobManager CreateBlobManager(string blobBucket)
    {
        IBlobStorageProvider[] blobStorageProviders = GetBlobStorageProviders<IBlobStorageProvider>(blobBucket);

        BlobManager blobManager = new(blobStorageProviders);

        return blobManager;
    }

    public IBlobManager<TBlobStorageProvider> CreateBlobManager<TBlobStorageProvider>()
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        TBlobStorageProvider[] blobStorageProviders = GetBlobStorageProviders<TBlobStorageProvider>();

        BlobManager<TBlobStorageProvider> blobManager = new(blobStorageProviders);

        return blobManager;
    }

    public IBlobManager<TBlobStorageProvider> CreateBlobManager<TBlobStorageProvider>(string blobBucket)
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        TBlobStorageProvider[] blobStorageProviders = GetBlobStorageProviders<TBlobStorageProvider>(blobBucket);

        BlobManager<TBlobStorageProvider> blobManager = new(blobStorageProviders);

        return blobManager;
    }

    private TBlobStorageProvider[] GetBlobStorageProviders<TBlobStorageProvider>(string? blobBucket = null)
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        IEnumerable<TBlobStorageProvider> blobStorageProvidersEnumerable =
            m_serviceProvider.GetRequiredService<IEnumerable<TBlobStorageProvider>>();

        if (blobBucket is not null)
        {
            blobStorageProvidersEnumerable =
                blobStorageProvidersEnumerable.Where(blobStorageProvider => blobStorageProvider.HasBlobBucket(blobBucket));
        }

        TBlobStorageProvider[] blobStorageProviders = [.. blobStorageProvidersEnumerable];

        return blobStorageProviders;
    }
}
