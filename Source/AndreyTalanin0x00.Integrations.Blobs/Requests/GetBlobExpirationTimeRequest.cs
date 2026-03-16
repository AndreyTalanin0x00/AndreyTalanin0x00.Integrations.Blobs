using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class GetBlobExpirationTimeRequest
{
    public required BlobReference BlobReference { get; set; }

    public GetBlobExpirationTimeRequest()
    {
    }

    [SetsRequiredMembers]
    public GetBlobExpirationTimeRequest(BlobReference blobReference)
    {
        BlobReference = blobReference;
    }
}
