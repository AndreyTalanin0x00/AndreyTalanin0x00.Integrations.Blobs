using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AndreyTalanin0x00.Integrations.Blobs;

public sealed class BlobStreamMetadataPair<TStream, TMetadata>
    where TStream : Stream
    where TMetadata : class
{
    public required TStream Stream { get; set; }

    public required TMetadata Metadata { get; set; }

    public BlobStreamMetadataPair()
    {
    }

    [SetsRequiredMembers]
    public BlobStreamMetadataPair(TStream stream, TMetadata metadata)
    {
        Stream = stream;
        Metadata = metadata;
    }

    public void Deconstruct(out TStream stream, out TMetadata metadata)
    {
        (stream, metadata) = (Stream, Metadata);
    }
}
