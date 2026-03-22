using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class DownloadBlobRequest
{
    public required BlobReference BlobReference { get; set; }

    public DownloadBlobRequest()
    {
    }

    [SetsRequiredMembers]
    public DownloadBlobRequest(BlobReference blobReference)
    {
        BlobReference = blobReference;
    }
}
