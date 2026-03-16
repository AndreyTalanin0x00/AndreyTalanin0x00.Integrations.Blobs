using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace AndreyTalanin0x00.Integrations.Blobs;

public sealed class DisposableBlobStreamMetadataPair<TStream, TMetadata> : IAsyncDisposable, IDisposable
    where TStream : Stream
    where TMetadata : class
{
    public required TStream Stream { get; init; }

    public required TMetadata Metadata { get; init; }

    public DisposableBlobStreamMetadataPair()
    {
    }

    [SetsRequiredMembers]
    public DisposableBlobStreamMetadataPair(TStream stream, TMetadata metadata)
    {
        Stream = stream;
        Metadata = metadata;
    }

    [SetsRequiredMembers]
    public DisposableBlobStreamMetadataPair(BlobStreamMetadataPair<TStream, TMetadata> blobStreamMetadataPair)
        : this(blobStreamMetadataPair.Stream, blobStreamMetadataPair.Metadata)
    {
    }

    public void Deconstruct(out TStream stream, out TMetadata metadata)
    {
        (stream, metadata) = (Stream, Metadata);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Stream.Dispose();
    }
}
