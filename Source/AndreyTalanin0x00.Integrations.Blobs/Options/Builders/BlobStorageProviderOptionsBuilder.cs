using System;
using System.Collections.Generic;

using AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AndreyTalanin0x00.Integrations.Blobs.Options.Builders;

public class BlobStorageProviderOptionsBuilder<TBlobStorageProvider> : IBlobStorageProviderOptionsBuilderInternal<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
    public IServiceCollection Services => throw new NotImplementedException();

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AddBlobBucket(string blobBucket)
    {
        throw new NotImplementedException();
    }

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AddBlobBuckets(IEnumerable<string> blobBucket)
    {
        throw new NotImplementedException();
    }

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> ClearBlobBuckets()
    {
        throw new NotImplementedException();
    }

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AllowBlobOperation(BlobOperation blobOperation, bool allowed = true)
    {
        throw new NotImplementedException();
    }

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AllowBlobOperations(IEnumerable<BlobOperation> blobOperations, bool allowed = true)
    {
        throw new NotImplementedException();
    }

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> ClearAllowedBlobOperations()
    {
        throw new NotImplementedException();
    }

    public BlobStorageProviderOptions<TBlobStorageProvider> Build()
    {
        throw new NotImplementedException();
    }
}
