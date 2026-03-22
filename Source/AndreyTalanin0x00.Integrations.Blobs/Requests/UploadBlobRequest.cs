using System;
using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class UploadBlobRequest
{
    public required string BlobBucket { get; set; }

    public required BlobMetadata BlobMetadata { get; set; }

    public required TimeSpan? BlobExpiresIn { get; set; }

    public UploadBlobRequest()
    {
    }

    [SetsRequiredMembers]
    public UploadBlobRequest(string blobBucket, BlobMetadata blobMetadata, TimeSpan? blobExpiresIn)
    {
        BlobBucket = blobBucket;
        BlobMetadata = blobMetadata;
        BlobExpiresIn = blobExpiresIn;
    }
}
