using System;

using AndreyTalanin0x00.Integrations.Blobs.Requests;

namespace AndreyTalanin0x00.Integrations.Blobs.Responses;

public class GetBlobExpirationTimeResponse
{
    public required DateTimeOffset? BlobExpirationTime { get; set; }

    public required GetBlobExpirationTimeRequest Request { get; set; }
}
