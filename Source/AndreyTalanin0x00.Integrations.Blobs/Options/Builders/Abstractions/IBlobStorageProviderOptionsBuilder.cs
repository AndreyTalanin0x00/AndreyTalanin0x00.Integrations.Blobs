using System.Collections.Generic;

using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AndreyTalanin0x00.Integrations.Blobs.Options.Builders.Abstractions;

public interface IBlobStorageProviderOptionsBuilder<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
    public IServiceCollection Services { get; }

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AddBlobBucket(string blobBucket);

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AddBlobBuckets(IEnumerable<string> blobBucket);

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> ClearBlobBuckets();

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AllowBlobOperation(BlobOperation blobOperation, bool allowed = true);

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> AllowBlobOperations(IEnumerable<BlobOperation> blobOperations, bool allowed = true);

    public IBlobStorageProviderOptionsBuilder<TBlobStorageProvider> ClearAllowedBlobOperations();
}

internal interface IBlobStorageProviderOptionsBuilderInternal<TBlobStorageProvider> : IBlobStorageProviderOptionsBuilder<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
    public BlobStorageProviderOptions<TBlobStorageProvider> Build();
}
