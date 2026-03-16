using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

namespace AndreyTalanin0x00.Integrations.Blobs.Services;

public class BlobManager : IBlobManager
{
    private readonly IBlobStorageProvider[] m_blobStorageProviders;
    private readonly Dictionary<string, IBlobStorageProvider[]> m_blobStorageProvidersByUriSchemes;

    public BlobManager(IEnumerable<IBlobStorageProvider> blobStorageProviders)
    {
        m_blobStorageProviders = [.. blobStorageProviders];
        m_blobStorageProvidersByUriSchemes = BuildBlobStorageProviderDictionary(blobStorageProviders);
    }

    /// <inheritdoc />
    public async Task<UploadBlobResponse> UploadBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair, CancellationToken cancellationToken = default)
    {
        UploadBlobRequest uploadBlobRequest = uploadBlobRequestPair.Metadata;

        string blobBucket = uploadBlobRequest.BlobBucket;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobBucket);

        async Task<UploadBlobResponse> UploadBlobCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.UploadBlobAsync(uploadBlobRequestPair, cancellationToken);

        UploadBlobResponse uploadBlobResponse =
            await ExecuteBlobOperationAsync(blobStorageProvider, UploadBlobCoreAsync, BlobOperation.Upload, cancellationToken);

        return uploadBlobResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryDownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, Action<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> setDownloadBlobResponsePairCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = downloadBlobRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<bool> TryDownloadBlobCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.TryDownloadBlobAsync(downloadBlobRequest, setDownloadBlobResponsePairCallback, cancellationToken);

        bool result = await ExecuteBlobOperationAsync(blobStorageProvider, TryDownloadBlobCoreAsync, BlobOperation.Download, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> DownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = downloadBlobRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> DownloadBlobCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.DownloadBlobAsync(downloadBlobRequest, cancellationToken);

        BlobStreamMetadataPair<Stream, DownloadBlobResponse> downloadBlobResponsePair =
            await ExecuteBlobOperationAsync(blobStorageProvider, DownloadBlobCoreAsync, BlobOperation.Download, cancellationToken);

        return downloadBlobResponsePair;
    }

    /// <inheritdoc />
    public async Task<bool> TryGetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, Action<GetBlobMetadataResponse> setGetBlobMetadataResponseCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = getBlobMetadataRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<bool> TryGetBlobMetadataCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.TryGetBlobMetadataAsync(getBlobMetadataRequest, setGetBlobMetadataResponseCallback, cancellationToken);

        bool result = await ExecuteBlobOperationAsync(blobStorageProvider, TryGetBlobMetadataCoreAsync, BlobOperation.GetMetadata, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<GetBlobMetadataResponse> GetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = getBlobMetadataRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<GetBlobMetadataResponse> GetBlobMetadataCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.GetBlobMetadataAsync(getBlobMetadataRequest, cancellationToken);

        GetBlobMetadataResponse getBlobMetadataResponse = await ExecuteBlobOperationAsync(blobStorageProvider, GetBlobMetadataCoreAsync, BlobOperation.GetMetadata, cancellationToken);

        return getBlobMetadataResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryGetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, Action<GetBlobExpirationTimeResponse> setGetBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = getBlobExpirationTimeRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<bool> TryGetBlobExpirationTimeCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.TryGetBlobExpirationTimeAsync(getBlobExpirationTimeRequest, setGetBlobExpirationTimeResponseCallback, cancellationToken);

        bool result = await ExecuteBlobOperationAsync(blobStorageProvider, TryGetBlobExpirationTimeCoreAsync, BlobOperation.GetExpirationTime, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<GetBlobExpirationTimeResponse> GetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = getBlobExpirationTimeRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<GetBlobExpirationTimeResponse> GetBlobExpirationTimeCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.GetBlobExpirationTimeAsync(getBlobExpirationTimeRequest, cancellationToken);

        GetBlobExpirationTimeResponse getBlobExpirationTimeResponse = await ExecuteBlobOperationAsync(blobStorageProvider, GetBlobExpirationTimeCoreAsync, BlobOperation.GetExpirationTime, cancellationToken);

        return getBlobExpirationTimeResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryUpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, Action<UpdateBlobMetadataResponse> setUpdateBlobMetadataResponseCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = updateBlobMetadataRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<bool> TryUpdateBlobMetadataCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.TryUpdateBlobMetadataAsync(updateBlobMetadataRequest, setUpdateBlobMetadataResponseCallback, cancellationToken);

        bool result = await ExecuteBlobOperationAsync(blobStorageProvider, TryUpdateBlobMetadataCoreAsync, BlobOperation.UpdateMetadata, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<UpdateBlobMetadataResponse> UpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = updateBlobMetadataRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<UpdateBlobMetadataResponse> UpdateBlobMetadataCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.UpdateBlobMetadataAsync(updateBlobMetadataRequest, cancellationToken);

        UpdateBlobMetadataResponse updateBlobMetadataResponse = await ExecuteBlobOperationAsync(blobStorageProvider, UpdateBlobMetadataCoreAsync, BlobOperation.UpdateMetadata, cancellationToken);

        return updateBlobMetadataResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryUpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, Action<UpdateBlobExpirationTimeResponse> setUpdateBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = updateBlobExpirationTimeRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<bool> TryUpdateBlobExpirationTimeCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.TryUpdateBlobExpirationTimeAsync(updateBlobExpirationTimeRequest, setUpdateBlobExpirationTimeResponseCallback, cancellationToken);

        bool result = await ExecuteBlobOperationAsync(blobStorageProvider, TryUpdateBlobExpirationTimeCoreAsync, BlobOperation.UpdateExpirationTime, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<UpdateBlobExpirationTimeResponse> UpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = updateBlobExpirationTimeRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<UpdateBlobExpirationTimeResponse> UpdateBlobExpirationTimeCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.UpdateBlobExpirationTimeAsync(updateBlobExpirationTimeRequest, cancellationToken);

        UpdateBlobExpirationTimeResponse updateBlobExpirationTimeResponse = await ExecuteBlobOperationAsync(blobStorageProvider, UpdateBlobExpirationTimeCoreAsync, BlobOperation.UpdateExpirationTime, cancellationToken);

        return updateBlobExpirationTimeResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryDeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, Action<DeleteBlobResponse> setDeleteBlobResponseCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = deleteBlobRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<bool> TryDeleteBlobCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.TryDeleteBlobAsync(deleteBlobRequest, setDeleteBlobResponseCallback, cancellationToken);

        bool result = await ExecuteBlobOperationAsync(blobStorageProvider, TryDeleteBlobCoreAsync, BlobOperation.Delete, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<DeleteBlobResponse> DeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = deleteBlobRequest.BlobReference;

        IBlobStorageProvider blobStorageProvider = ResolveBlobStorageProvider(blobReference);

        async Task<DeleteBlobResponse> DeleteBlobCoreAsync(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken) =>
            await blobStorageProvider.DeleteBlobAsync(deleteBlobRequest, cancellationToken);

        DeleteBlobResponse deleteBlobResponse = await ExecuteBlobOperationAsync(blobStorageProvider, DeleteBlobCoreAsync, BlobOperation.Delete, cancellationToken);

        return deleteBlobResponse;
    }

    private static Dictionary<string, IBlobStorageProvider[]> BuildBlobStorageProviderDictionary(IEnumerable<IBlobStorageProvider> blobStorageProviders)
    {
        static string KeySelector(IGrouping<string, IBlobStorageProvider> blobStorageProviderGrouping) => blobStorageProviderGrouping.Key;

        static IBlobStorageProvider[] ElementSelector(IGrouping<string, IBlobStorageProvider> blobStorageProviderGrouping) => [.. blobStorageProviderGrouping];

        Dictionary<string, IBlobStorageProvider[]> blobStorageProvidersByUriSchemes = blobStorageProviders
            .GroupBy(blobStorageProvider => blobStorageProvider.UriScheme)
            .ToDictionary(KeySelector, ElementSelector);

        return blobStorageProvidersByUriSchemes;
    }

    private static InvalidOperationException CreateNoSuitableBlobStorageProviderException(string blobBucket)
    {
        const string exceptionMessageFormat = "No suitable blob storage provider is configured. Blob bucket: {0}. Check blob services (blob storage provider configuration).";

        string exceptionMessage = string.Format(exceptionMessageFormat, blobBucket);

        InvalidOperationException exception = new(exceptionMessage);

        return exception;
    }

    private static InvalidOperationException CreateNoSuitableBlobStorageProviderException(Uri blobUri)
    {
        const string exceptionMessageFormat = "No suitable blob storage provider is configured. Blob URI: {0}. Check blob services (blob storage provider configuration).";

        string exceptionMessage = string.Format(exceptionMessageFormat, blobUri);

        InvalidOperationException exception = new(exceptionMessage);

        return exception;
    }

    private static InvalidOperationException CreateNoUnambiguousBlobStorageProviderException(string blobBucket)
    {
        const string exceptionMessageFormat = "No unambiguous blob storage provider is configured. Blob bucket: {0}. Check blob services (blob storage provider configuration) or use a type-parameterized blob manager.";

        string exceptionMessage = string.Format(exceptionMessageFormat, blobBucket);

        InvalidOperationException exception = new(exceptionMessage);

        return exception;
    }

    private static InvalidOperationException CreateNoUnambiguousBlobStorageProviderException(Uri blobUri)
    {
        const string exceptionMessageFormat = "No unambiguous blob storage provider is configured. Blob URI: {0}. Check blob services (blob storage provider configuration) or use a type-parameterized blob manager.";

        string exceptionMessage = string.Format(exceptionMessageFormat, blobUri);

        InvalidOperationException exception = new(exceptionMessage);

        return exception;
    }

    private static InvalidOperationException CreateBlobOperationNotSupportedException(BlobOperation blobOperation, IBlobStorageProvider blobStorageProvider)
    {
        string uriScheme = blobStorageProvider.UriScheme;

        const string exceptionMessageFormat = "Can not perform the {0} operation on the blob storage provider (URI scheme: {1}) because it is not supported.";

        string exceptionMessage = string.Format(exceptionMessageFormat, blobOperation, uriScheme);

        InvalidOperationException exception = new(exceptionMessage);

        return exception;
    }

    private static InvalidOperationException CreateBlobOperationNotAllowedException(BlobOperation blobOperation, IBlobStorageProvider blobStorageProvider)
    {
        string uriScheme = blobStorageProvider.UriScheme;

        const string exceptionMessageFormat = "Can not perform the {0} operation on the blob storage provider (URI scheme: {1}) because it is not allowed.";

        string exceptionMessage = string.Format(exceptionMessageFormat, blobOperation, uriScheme);

        InvalidOperationException exception = new(exceptionMessage);

        return exception;
    }

    private static async Task<TResult> ExecuteBlobOperationAsync<TResult>(IBlobStorageProvider blobStorageProvider, BlobStorageProviderAsyncFunction<TResult> blobStorageProviderAsyncFunction, BlobOperation blobOperation, CancellationToken cancellationToken)
    {
        if (!blobStorageProvider.SupportsBlobOperation(blobOperation))
            throw CreateBlobOperationNotSupportedException(blobOperation, blobStorageProvider);
        if (!blobStorageProvider.AllowsBlobOperation(blobOperation))
            throw CreateBlobOperationNotAllowedException(blobOperation, blobStorageProvider);

        TResult result = await blobStorageProviderAsyncFunction(blobStorageProvider, cancellationToken);

        return result;
    }

    private IBlobStorageProvider ResolveBlobStorageProvider(string blobBucket)
    {
        IBlobStorageProvider? blobStorageProvider = null;

        int matchingBlobStorageProvidersCount = 0;
        IEnumerable<IBlobStorageProvider> blobStorageProvidersToCheck = m_blobStorageProviders;
        foreach (IBlobStorageProvider blobStorageProviderToCheck in blobStorageProvidersToCheck)
        {
            if (!blobStorageProviderToCheck.HasBlobBucket(blobBucket))
                continue;

            blobStorageProvider = blobStorageProviderToCheck;

            matchingBlobStorageProvidersCount++;
        }

#pragma warning disable format
        if (blobStorageProvider is null || matchingBlobStorageProvidersCount == 0)
            throw CreateNoSuitableBlobStorageProviderException(blobBucket);
        if (matchingBlobStorageProvidersCount > 1)
            throw CreateNoUnambiguousBlobStorageProviderException(blobBucket);
#pragma warning restore format

        return blobStorageProvider;
    }

    private IBlobStorageProvider ResolveBlobStorageProvider(BlobReference blobReference)
    {
        Uri blobUri = blobReference.Uri;

        string blobUriScheme = blobUri.Scheme;

        IBlobStorageProvider? blobStorageProvider = null;

        if (!m_blobStorageProvidersByUriSchemes.TryGetValue(blobUriScheme, out IBlobStorageProvider[]? blobStorageProvidersToCheck))
            throw CreateNoSuitableBlobStorageProviderException(blobUri);

        if (blobStorageProvidersToCheck.Length == 0)
            throw CreateNoSuitableBlobStorageProviderException(blobUri);

        if (blobStorageProvidersToCheck.Length > 1)
            throw CreateNoUnambiguousBlobStorageProviderException(blobUri);

        blobStorageProvider = blobStorageProvidersToCheck.Single();

        return blobStorageProvider;
    }

    private delegate Task<TResult> BlobStorageProviderAsyncFunction<TResult>(IBlobStorageProvider blobStorageProvider, CancellationToken cancellationToken);
}

public class BlobManager<TBlobStorageProvider> : BlobManager, IBlobManager<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
    public BlobManager(IEnumerable<TBlobStorageProvider> blobStorageProviders)
        : base(blobStorageProviders)
    {
    }
}
