using AndreyTalanin0x00.Integrations.Blobs.Requests;

namespace AndreyTalanin0x00.Integrations.Blobs.Responses;

public class GetBlobMetadataResponse
{
    public required BlobMetadata BlobMetadata { get; set; }

    public required GetBlobMetadataRequest Request { get; set; }
}
