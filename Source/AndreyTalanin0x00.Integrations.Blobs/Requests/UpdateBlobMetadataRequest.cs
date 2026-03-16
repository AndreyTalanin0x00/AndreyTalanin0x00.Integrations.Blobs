using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class UpdateBlobMetadataRequest
{
    public required BlobReference BlobReference { get; set; }

    public required BlobMetadata BlobMetadata { get; set; }

    public UpdateBlobMetadataRequest()
    {
    }

    [SetsRequiredMembers]
    public UpdateBlobMetadataRequest(BlobReference blobReference, BlobMetadata blobMetadata)
    {
        BlobReference = blobReference;
        BlobMetadata = blobMetadata;
    }
}
