using System.IO;

namespace AndreyTalanin0x00.Integrations.Blobs.Extensions;

public static class BlobStreamMetadataPairExtensions
{
    public static DisposableBlobStreamMetadataPair<TStream, TMetadata> AsDisposable<TStream, TMetadata>(this BlobStreamMetadataPair<TStream, TMetadata> blobStreamMetadataPair)
        where TStream : Stream
        where TMetadata : class
    {
        DisposableBlobStreamMetadataPair<TStream, TMetadata> disposableBlobStreamMetadataPair = new(blobStreamMetadataPair);

        return disposableBlobStreamMetadataPair;
    }
}
