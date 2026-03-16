using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

namespace AndreyTalanin0x00.Integrations.Blobs.Services;

public class BlobManager : IBlobManager
{
    /// <inheritdoc />
    public Task<UploadBlobResponse> UploadBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> TryDownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, Action<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> setDownloadBlobResponsePairCallback, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> DownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> TryGetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, Action<GetBlobMetadataResponse> setGetBlobMetadataResponseCallback, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<GetBlobMetadataResponse> GetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> TryGetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, Action<GetBlobExpirationTimeResponse> setGetBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<GetBlobExpirationTimeResponse> GetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> TryUpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, Action<UpdateBlobMetadataResponse> setUpdateBlobMetadataResponseCallback, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<UpdateBlobMetadataResponse> UpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> TryUpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, Action<UpdateBlobExpirationTimeResponse> setUpdateBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<UpdateBlobExpirationTimeResponse> UpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> TryDeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, Action<DeleteBlobResponse> setDeleteBlobResponseCallback, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<DeleteBlobResponse> DeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class BlobManager<TBlobStorageProvider> : BlobManager, IBlobManager<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
}
