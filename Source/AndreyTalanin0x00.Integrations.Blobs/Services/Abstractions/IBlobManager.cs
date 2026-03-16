using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;

namespace AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

public interface IBlobManager
{
    public Task<UploadBlobResponse> UploadBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair, CancellationToken cancellationToken = default);

    public Task<bool> TryDownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, Action<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> setDownloadBlobResponsePairCallback, CancellationToken cancellationToken = default);

    public Task<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> DownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, CancellationToken cancellationToken = default);

    public Task<bool> TryGetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, Action<GetBlobMetadataResponse> setGetBlobMetadataResponseCallback, CancellationToken cancellationToken = default);

    public Task<GetBlobMetadataResponse> GetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, CancellationToken cancellationToken = default);

    public Task<bool> TryGetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, Action<GetBlobExpirationTimeResponse> setGetBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default);

    public Task<GetBlobExpirationTimeResponse> GetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, CancellationToken cancellationToken = default);

    public Task<bool> TryUpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, Action<UpdateBlobMetadataResponse> setUpdateBlobMetadataResponseCallback, CancellationToken cancellationToken = default);

    public Task<UpdateBlobMetadataResponse> UpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, CancellationToken cancellationToken = default);

    public Task<bool> TryUpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, Action<UpdateBlobExpirationTimeResponse> setUpdateBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default);

    public Task<UpdateBlobExpirationTimeResponse> UpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, CancellationToken cancellationToken = default);

    public Task<bool> TryDeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, Action<DeleteBlobResponse> setDeleteBlobResponseCallback, CancellationToken cancellationToken = default);

    public Task<DeleteBlobResponse> DeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, CancellationToken cancellationToken = default);
}

public interface IBlobManager<TBlobStorageProvider> : IBlobManager
    where TBlobStorageProvider : class, IBlobStorageProvider
{
}
