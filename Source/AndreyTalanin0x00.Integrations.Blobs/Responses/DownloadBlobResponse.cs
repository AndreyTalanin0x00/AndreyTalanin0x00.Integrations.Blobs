using AndreyTalanin0x00.Integrations.Blobs.Requests;

namespace AndreyTalanin0x00.Integrations.Blobs.Responses;

public class DownloadBlobResponse
{
    public required BlobMetadata BlobMetadata { get; set; }

    public required DownloadBlobRequest Request { get; set; }
}
