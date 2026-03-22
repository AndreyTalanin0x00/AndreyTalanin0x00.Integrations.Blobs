using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class GetBlobMetadataRequest
{
    public required BlobReference BlobReference { get; set; }

    public GetBlobMetadataRequest()
    {
    }

    [SetsRequiredMembers]
    public GetBlobMetadataRequest(BlobReference blobReference)
    {
        BlobReference = blobReference;
    }
}
