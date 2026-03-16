using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

public interface IBlobStorageProvider : IBlobManager
{
    public string UriScheme { get; }

    public static virtual IReadOnlyCollection<BlobOperation> GetDefaultSupportedBlobOperations()
    {
        return new ReadOnlyCollection<BlobOperation>([]);
    }

    public bool HasBlobBucket(string blobBucket);

    public bool SupportsBlobOperation(BlobOperation blobOperation);

    public bool AllowsBlobOperation(BlobOperation blobOperation);
}
