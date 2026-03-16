using System;

using AndreyTalanin0x00.Integrations.Blobs.Requests;

namespace AndreyTalanin0x00.Integrations.Blobs.Responses;

public class UploadBlobResponse
{
    public required BlobReference BlobReference { get; set; }

    public required DateTimeOffset BlobUploadedOn { get; set; }

    public required DateTimeOffset? BlobExpiresOn { get; set; }

    public required UploadBlobRequest Request { get; set; }
}
