using System;
using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs.Requests;

public class UpdateBlobExpirationTimeRequest
{
    public required BlobReference BlobReference { get; set; }

    public required TimeSpan? BlobExpiresIn { get; set; }

    public UpdateBlobExpirationTimeRequest()
    {
    }

    [SetsRequiredMembers]
    public UpdateBlobExpirationTimeRequest(BlobReference blobReference, TimeSpan? blobExpiresIn)
    {
        BlobReference = blobReference;
        BlobExpiresIn = blobExpiresIn;
    }
}
