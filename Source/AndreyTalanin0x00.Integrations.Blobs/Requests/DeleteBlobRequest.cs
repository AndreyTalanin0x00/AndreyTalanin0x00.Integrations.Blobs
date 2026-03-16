using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class DeleteBlobRequest
{
    public required BlobReference BlobReference { get; set; }

    public DeleteBlobRequest()
    {
    }

    [SetsRequiredMembers]
    public DeleteBlobRequest(BlobReference blobReference)
    {
        BlobReference = blobReference;
    }
}
