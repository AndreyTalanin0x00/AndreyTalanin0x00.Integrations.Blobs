namespace AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

public interface IBlobManagerFactory
{
    public IBlobManager CreateBlobManager();

    public IBlobManager CreateBlobManager(string blobBucket);

    public IBlobManager<TBlobStorageProvider> CreateBlobManager<TBlobStorageProvider>()
        where TBlobStorageProvider : class, IBlobStorageProvider;

    public IBlobManager<TBlobStorageProvider> CreateBlobManager<TBlobStorageProvider>(string blobBucket)
        where TBlobStorageProvider : class, IBlobStorageProvider;
}
