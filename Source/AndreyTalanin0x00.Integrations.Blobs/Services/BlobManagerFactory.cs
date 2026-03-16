using System;

using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

namespace AndreyTalanin0x00.Integrations.Blobs.Services;

public class BlobManagerFactory : IBlobManagerFactory
{
    public IBlobManager CreateBlobManager()
    {
        throw new NotImplementedException();
    }

    public IBlobManager CreateBlobManager(string blobBucket)
    {
        throw new NotImplementedException();
    }

    public IBlobManager<TBlobStorageProvider> CreateBlobManager<TBlobStorageProvider>()
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        throw new NotImplementedException();
    }

    public IBlobManager<TBlobStorageProvider> CreateBlobManager<TBlobStorageProvider>(string blobBucket)
        where TBlobStorageProvider : class, IBlobStorageProvider
    {
        throw new NotImplementedException();
    }
}
