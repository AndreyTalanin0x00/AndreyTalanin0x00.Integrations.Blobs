using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs.Configuration;
using AndreyTalanin0x00.Integrations.Blobs.Options;
using AndreyTalanin0x00.Integrations.Blobs.Requests;
using AndreyTalanin0x00.Integrations.Blobs.Responses;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IO;

// Disable the CS1998 (Async method lacks 'await' operators and will run synchronously) warning to make the methods async even when not necessary.
// This helps to avoid exceptions being thrown at the Task creation moment instead of the awaiting point.
#pragma warning disable CS1998

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace AndreyTalanin0x00.Integrations.Blobs.Services;

public class InMemoryBlobStorageProvider : BackgroundService, IBlobStorageProvider
{
    // The virtual URI looks like this:
    // in-memory://in-memory-blob-storage.localhost/global/00000000-0000-0000-0000-000000000000
    private const string c_uriScheme = "in-memory";
    private const string c_uriHost = "in-memory-blob-storage.localhost";
    private const string c_uriDirectory = "global";

    private static readonly TimeSpan s_expiredBlobCleanUpPeriod = TimeSpan.FromMinutes(5);
    private static readonly ReadOnlyCollection<BlobOperation> s_defaultSupportedBlobOperations = BuildDefaultSupportedBlobOperationReadOnlyCollection();
    private static readonly HashSet<BlobOperation> s_defaultSupportedBlobOperationsHashSet = BuildDefaultSupportedBlobOperationHashSet();

    private readonly TimeProvider m_timeProvider;
    private readonly RecyclableMemoryStreamManager m_recyclableMemoryStreamManager;
    private readonly BlobStorageProviderOptions<InMemoryBlobStorageProvider> m_blobStorageProviderOptions;
    private readonly InMemoryBlobStorageProviderConfiguration m_blobStorageProviderConfiguration;
    private readonly HashSet<string> m_blobBucketHashSet;
    private readonly HashSet<BlobOperation> m_allowedBlobOperationHashSet;
    private readonly ConcurrentDictionary<Guid, BlobInfo> m_blobInfoObjectsByIds;

    /// <inheritdoc />
    public string UriScheme { get; } = c_uriScheme;

    public InMemoryBlobStorageProvider(TimeProvider timeProvider, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IOptions<BlobStorageProviderOptions<InMemoryBlobStorageProvider>> blobStorageProviderOptionsWrapper, IOptions<InMemoryBlobStorageProviderConfiguration> blobStorageProviderConfigurationWrapper)
    {
        m_timeProvider = timeProvider;
        m_recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        m_blobStorageProviderOptions = blobStorageProviderOptionsWrapper.Value;
        m_blobStorageProviderConfiguration = blobStorageProviderConfigurationWrapper.Value;
        m_blobBucketHashSet = BuildBlobBucketHashSet(m_blobStorageProviderOptions);
        m_allowedBlobOperationHashSet = BuildAllowedBlobOperationHashSet(m_blobStorageProviderOptions);
        m_blobInfoObjectsByIds = new ConcurrentDictionary<Guid, BlobInfo>();
    }

    /// <inheritdoc />
    public static IReadOnlyCollection<BlobOperation> GetDefaultSupportedBlobOperations()
    {
        return s_defaultSupportedBlobOperations;
    }

    /// <inheritdoc />
    public bool HasBlobBucket(string blobBucket)
    {
        HashSet<string> blobBuckets = m_blobBucketHashSet;
        bool hasBlobBucket = blobBuckets.Contains(blobBucket);
        return hasBlobBucket;
    }

    /// <inheritdoc />
    public bool SupportsBlobOperation(BlobOperation blobOperation)
    {
        HashSet<BlobOperation> supportedBlobOperations = s_defaultSupportedBlobOperationsHashSet;
        bool supportsBlobOperation = supportedBlobOperations.Contains(blobOperation);
        return supportsBlobOperation;
    }

    /// <inheritdoc />
    public bool AllowsBlobOperation(BlobOperation blobOperation)
    {
        HashSet<BlobOperation> allowedBlobOperations = m_allowedBlobOperationHashSet;
        bool allowsBlobOperation = allowedBlobOperations.Contains(blobOperation);
        return allowsBlobOperation;
    }

    /// <inheritdoc />
    public async Task<UploadBlobResponse> UploadBlobAsync(BlobStreamMetadataPair<Stream, UploadBlobRequest> uploadBlobRequestPair, CancellationToken cancellationToken = default)
    {
        (Stream blobSourceStream, UploadBlobRequest uploadBlobRequest) = uploadBlobRequestPair;

        string blobBucket = uploadBlobRequest.BlobBucket;

        ThrowIfBlobBucketIsNotConfigured(blobBucket);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.Upload);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.Upload);

        Guid blobId = Guid.NewGuid();

        Uri blobUri = BuildBlobUri(blobId);

        BlobReference blobReference = new(blobId, blobUri);
        BlobMetadata blobMetadata = uploadBlobRequest.BlobMetadata;

        TimeSpan? blobExpiresIn = uploadBlobRequest.BlobExpiresIn;
        DateTimeOffset blobUploadedOn = m_timeProvider.GetUtcNow();
        DateTimeOffset? blobExpiresOn = blobExpiresIn is not null
            ? blobUploadedOn + blobExpiresIn
            : null;

        MemoryStream blobStorageMemoryStream = new();

        try
        {
            await blobSourceStream.CopyToAsync(blobStorageMemoryStream, cancellationToken);

            blobStorageMemoryStream.Seek(0L, SeekOrigin.Begin);

            BlobInfo blobInfoObject = new()
            {
                Bucket = blobBucket,
                Reference = blobReference,
                Metadata = blobMetadata,
                ExpiresOn = blobExpiresOn,
                MemoryStream = blobStorageMemoryStream,
            };

            if (!m_blobInfoObjectsByIds.TryAdd(blobId, blobInfoObject))
                throw new UnreachableException("Can not upload a blob due to a GUID collision (realistically impossible).");

            UploadBlobResponse uploadBlobResponse = new()
            {
                BlobReference = blobReference,
                BlobUploadedOn = blobUploadedOn,
                BlobExpiresOn = blobExpiresOn,
                Request = uploadBlobRequest,
            };

            return uploadBlobResponse;
        }
        catch (Exception)
        {
            blobStorageMemoryStream.Dispose();

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryDownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, Action<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> setDownloadBlobResponsePairCallback, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = downloadBlobRequest.BlobReference;

        (Guid blobId, Uri blobUri) = blobReference;

        ThrowIfUriSchemeIsNotSupported(blobUri);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.Download);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.Download);

        if (!m_blobInfoObjectsByIds.TryGetValue(blobId, out BlobInfo? blobInfoObject))
            return false;

        BlobMetadata blobMetadata = blobInfoObject.Metadata;

        Stream blobStorageMemoryStream = blobInfoObject.MemoryStream;

        MemoryStream blobTargetStream = m_recyclableMemoryStreamManager.GetStream();

        try
        {
            await blobStorageMemoryStream.CopyToAsync(blobTargetStream, cancellationToken);

            blobStorageMemoryStream.Seek(0L, SeekOrigin.Begin);
            blobTargetStream.Seek(0L, SeekOrigin.Begin);

            DownloadBlobResponse downloadBlobResponse = new()
            {
                BlobMetadata = blobMetadata,
                Request = downloadBlobRequest,
            };

            BlobStreamMetadataPair<Stream, DownloadBlobResponse> downloadBlobResponsePair = new(blobTargetStream, downloadBlobResponse);

            setDownloadBlobResponsePairCallback(downloadBlobResponsePair);

            return true;
        }
        catch (Exception)
        {
            blobTargetStream.Dispose();

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<BlobStreamMetadataPair<Stream, DownloadBlobResponse>> DownloadBlobAsync(DownloadBlobRequest downloadBlobRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = downloadBlobRequest.BlobReference;

        BlobStreamMetadataPair<Stream, DownloadBlobResponse>? downloadBlobResponsePair = null;

        void SetDownloadBlobResponsePair(BlobStreamMetadataPair<Stream, DownloadBlobResponse> outDownloadBlobResponsePair) =>
            downloadBlobResponsePair = outDownloadBlobResponsePair;
        
#pragma warning disable format
        if (!await TryDownloadBlobAsync(downloadBlobRequest, SetDownloadBlobResponsePair, cancellationToken) || downloadBlobResponsePair is null)
            throw CreateBlobNotFoundException(blobReference);
#pragma warning restore format

        return downloadBlobResponsePair;
    }

    /// <inheritdoc />
    public async Task<bool> TryGetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, Action<GetBlobMetadataResponse> setGetBlobMetadataResponseCallback, CancellationToken cancellationToken = default)
    {
        (Guid blobId, Uri blobUri) = getBlobMetadataRequest.BlobReference;

        ThrowIfUriSchemeIsNotSupported(blobUri);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.GetMetadata);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.GetMetadata);

        if (!m_blobInfoObjectsByIds.TryGetValue(blobId, out BlobInfo? blobInfoObject))
            return false;

        BlobMetadata blobMetadata = blobInfoObject.Metadata;

        GetBlobMetadataResponse getBlobMetadataResponse = new()
        {
            BlobMetadata = blobMetadata,
            Request = getBlobMetadataRequest,
        };

        setGetBlobMetadataResponseCallback(getBlobMetadataResponse);

        return true;
    }

    /// <inheritdoc />
    public async Task<GetBlobMetadataResponse> GetBlobMetadataAsync(GetBlobMetadataRequest getBlobMetadataRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = getBlobMetadataRequest.BlobReference;

        GetBlobMetadataResponse? getBlobMetadataResponse = null;

        void SetGetBlobMetadataResponse(GetBlobMetadataResponse outGetBlobMetadataResponse) =>
            getBlobMetadataResponse = outGetBlobMetadataResponse;

#pragma warning disable format
        if (!await TryGetBlobMetadataAsync(getBlobMetadataRequest, SetGetBlobMetadataResponse, cancellationToken) || getBlobMetadataResponse is null)
            throw CreateBlobNotFoundException(blobReference);
#pragma warning restore format

        return getBlobMetadataResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryGetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, Action<GetBlobExpirationTimeResponse> setGetBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default)
    {
        (Guid blobId, Uri blobUri) = getBlobExpirationTimeRequest.BlobReference;

        ThrowIfUriSchemeIsNotSupported(blobUri);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.GetExpirationTime);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.GetExpirationTime);

        if (!m_blobInfoObjectsByIds.TryGetValue(blobId, out BlobInfo? blobInfoObject))
            return false;

        DateTimeOffset? blobExpiresOn = blobInfoObject.ExpiresOn;

        GetBlobExpirationTimeResponse getBlobExpirationTimeResponse = new()
        {
            BlobExpirationTime = blobExpiresOn,
            Request = getBlobExpirationTimeRequest,
        };

        setGetBlobExpirationTimeResponseCallback(getBlobExpirationTimeResponse);

        return true;
    }

    /// <inheritdoc />
    public async Task<GetBlobExpirationTimeResponse> GetBlobExpirationTimeAsync(GetBlobExpirationTimeRequest getBlobExpirationTimeRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = getBlobExpirationTimeRequest.BlobReference;

        GetBlobExpirationTimeResponse? getBlobExpirationTimeResponse = null;

        void SetGetBlobExpirationTimeResponse(GetBlobExpirationTimeResponse outGetBlobExpirationTimeResponse) =>
            getBlobExpirationTimeResponse = outGetBlobExpirationTimeResponse;

#pragma warning disable format
        if (!await TryGetBlobExpirationTimeAsync(getBlobExpirationTimeRequest, SetGetBlobExpirationTimeResponse, cancellationToken) || getBlobExpirationTimeResponse is null)
            throw CreateBlobNotFoundException(blobReference);
#pragma warning restore format

        return getBlobExpirationTimeResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryUpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, Action<UpdateBlobMetadataResponse> setUpdateBlobMetadataResponseCallback, CancellationToken cancellationToken = default)
    {
        (Guid blobId, Uri blobUri) = updateBlobMetadataRequest.BlobReference;

        ThrowIfUriSchemeIsNotSupported(blobUri);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.UpdateMetadata);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.UpdateMetadata);

        if (!m_blobInfoObjectsByIds.TryGetValue(blobId, out BlobInfo? blobInfoObject))
            return false;

        blobInfoObject.Metadata = updateBlobMetadataRequest.BlobMetadata;

        UpdateBlobMetadataResponse updateBlobMetadataResponse = new()
        {
            Request = updateBlobMetadataRequest,
        };

        setUpdateBlobMetadataResponseCallback(updateBlobMetadataResponse);

        return true;
    }

    /// <inheritdoc />
    public async Task<UpdateBlobMetadataResponse> UpdateBlobMetadataAsync(UpdateBlobMetadataRequest updateBlobMetadataRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = updateBlobMetadataRequest.BlobReference;

        UpdateBlobMetadataResponse? updateBlobMetadataResponse = null;

        void SetUpdateBlobMetadataResponse(UpdateBlobMetadataResponse outUpdateBlobMetadataResponse) =>
            updateBlobMetadataResponse = outUpdateBlobMetadataResponse;

#pragma warning disable format
        if (!await TryUpdateBlobMetadataAsync(updateBlobMetadataRequest, SetUpdateBlobMetadataResponse, cancellationToken) || updateBlobMetadataResponse is null)
            throw CreateBlobNotFoundException(blobReference);
#pragma warning restore format

        return updateBlobMetadataResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryUpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, Action<UpdateBlobExpirationTimeResponse> setUpdateBlobExpirationTimeResponseCallback, CancellationToken cancellationToken = default)
    {
        (Guid blobId, Uri blobUri) = updateBlobExpirationTimeRequest.BlobReference;

        ThrowIfUriSchemeIsNotSupported(blobUri);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.UpdateExpirationTime);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.UpdateExpirationTime);

        if (!m_blobInfoObjectsByIds.TryGetValue(blobId, out BlobInfo? blobInfoObject))
            return false;

        TimeSpan? blobExpiresIn = updateBlobExpirationTimeRequest.BlobExpiresIn;

        DateTimeOffset blobExpirationTimeUpdatedOn = m_timeProvider.GetUtcNow();
        DateTimeOffset? blobExpiresOn = blobExpiresIn is not null
            ? blobExpirationTimeUpdatedOn + blobExpiresIn
            : null;

        blobInfoObject.ExpiresOn = blobExpiresOn;

        UpdateBlobExpirationTimeResponse updateBlobExpirationTimeResponse = new()
        {
            Request = updateBlobExpirationTimeRequest,
        };

        setUpdateBlobExpirationTimeResponseCallback(updateBlobExpirationTimeResponse);

        return true;
    }

    /// <inheritdoc />
    public async Task<UpdateBlobExpirationTimeResponse> UpdateBlobExpirationTimeAsync(UpdateBlobExpirationTimeRequest updateBlobExpirationTimeRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = updateBlobExpirationTimeRequest.BlobReference;

        UpdateBlobExpirationTimeResponse? updateBlobExpirationTimeResponse = null;

        void SetUpdateBlobExpirationTimeResponse(UpdateBlobExpirationTimeResponse outUpdateBlobExpirationTimeResponse) =>
            updateBlobExpirationTimeResponse = outUpdateBlobExpirationTimeResponse;

#pragma warning disable format
        if (!await TryUpdateBlobExpirationTimeAsync(updateBlobExpirationTimeRequest, SetUpdateBlobExpirationTimeResponse, cancellationToken) || updateBlobExpirationTimeResponse is null)
            throw CreateBlobNotFoundException(blobReference);
#pragma warning restore format

        return updateBlobExpirationTimeResponse;
    }

    /// <inheritdoc />
    public async Task<bool> TryDeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, Action<DeleteBlobResponse> setDeleteBlobResponseCallback, CancellationToken cancellationToken = default)
    {
        (Guid blobId, Uri blobUri) = deleteBlobRequest.BlobReference;

        ThrowIfUriSchemeIsNotSupported(blobUri);
        ThrowIfBlobOperationIsNotSupported(BlobOperation.Delete);
        ThrowIfBlobOperationIsNotAllowed(BlobOperation.Delete);

        if (!m_blobInfoObjectsByIds.TryGetValue(blobId, out BlobInfo? blobInfoObject))
            return true;

        Stream blobStorageMemoryStream = blobInfoObject.MemoryStream;

        if (!m_blobInfoObjectsByIds.TryRemove(blobId, out blobInfoObject))
            return true;

        blobStorageMemoryStream.Dispose();

        DeleteBlobResponse deleteBlobResponse = new()
        {
            Request = deleteBlobRequest,
        };

        setDeleteBlobResponseCallback(deleteBlobResponse);

        return true;
    }

    /// <inheritdoc />
    public async Task<DeleteBlobResponse> DeleteBlobAsync(DeleteBlobRequest deleteBlobRequest, CancellationToken cancellationToken = default)
    {
        BlobReference blobReference = deleteBlobRequest.BlobReference;

        DeleteBlobResponse? deleteBlobResponse = null;

        void SetDeleteBlobResponse(DeleteBlobResponse outDeleteBlobResponse) =>
            deleteBlobResponse = outDeleteBlobResponse;

#pragma warning disable format
        if (!await TryDeleteBlobAsync(deleteBlobRequest, SetDeleteBlobResponse, cancellationToken) || deleteBlobResponse is null)
            throw new UnreachableException($"Can not delete the '{blobReference}' blob.");
#pragma warning restore format

        return deleteBlobResponse;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        TimeSpan expiredBlobCleanUpPeriod = s_expiredBlobCleanUpPeriod;
        if (m_blobStorageProviderConfiguration.ExpiredBlobCleanUpPeriod != default(TimeSpan))
            expiredBlobCleanUpPeriod = m_blobStorageProviderConfiguration.ExpiredBlobCleanUpPeriod;

        List<Guid> blobIdsToCleanUp = [];
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(expiredBlobCleanUpPeriod, stoppingToken);

            DateTimeOffset utcNow = m_timeProvider.GetUtcNow();
            foreach ((Guid blobId, BlobInfo blobInfoObject) in m_blobInfoObjectsByIds)
            {
                if (blobInfoObject.ExpiresOn is null || utcNow < (DateTimeOffset)blobInfoObject.ExpiresOn)
                    continue;

                blobIdsToCleanUp.Add(blobId);
            }

            foreach (Guid blobId in blobIdsToCleanUp)
                m_blobInfoObjectsByIds.Remove(blobId, out _);

            blobIdsToCleanUp.Clear();
        }
    }

    private static ReadOnlyCollection<BlobOperation> BuildDefaultSupportedBlobOperationReadOnlyCollection()
    {
        BlobOperation[] defaultSupportedBlobOperationsArray = Enum.GetValues<BlobOperation>()
            .Where(blobOperation => blobOperation != BlobOperation.Unknown)
            .ToArray();

        ReadOnlyCollection<BlobOperation> defaultSupportedBlobOperationsReadOnlyCollection = new(defaultSupportedBlobOperationsArray);

        return defaultSupportedBlobOperationsReadOnlyCollection;
    }

    private static HashSet<BlobOperation> BuildDefaultSupportedBlobOperationHashSet()
    {
        HashSet<BlobOperation> defaultSupportedBlobOperationsReadOnlyCollection = s_defaultSupportedBlobOperations.ToHashSet();

        return defaultSupportedBlobOperationsReadOnlyCollection;
    }

    private static HashSet<string> BuildBlobBucketHashSet(BlobStorageProviderOptions<InMemoryBlobStorageProvider> blobStorageProviderOptions)
    {
        HashSet<string> blobBuckets = blobStorageProviderOptions.BlobBuckets.ToHashSet();
        return blobBuckets;
    }

    private static HashSet<BlobOperation> BuildAllowedBlobOperationHashSet(BlobStorageProviderOptions<InMemoryBlobStorageProvider> blobStorageProviderOptions)
    {
        HashSet<BlobOperation> allowedBlobOperations = blobStorageProviderOptions.AllowedBlobOperations.ToHashSet();
        return allowedBlobOperations;
    }

    private static Uri BuildBlobUri(Guid blobId)
    {
        // The virtual URI looks like this:
        // in-memory://in-memory-blob-storage.localhost/global/00000000-0000-0000-0000-000000000000

        string path = Path.Combine(c_uriDirectory, blobId.ToString("D"));

        UriBuilder uriBuilder = new()
        {
            Scheme = c_uriScheme,
            Host = c_uriHost,
            Path = path,
        };

        Uri uri = uriBuilder.Uri;

        return uri;
    }

    private static void ThrowIfUriSchemeIsNotSupported(Uri blobUri)
    {
        if (blobUri.Scheme != c_uriScheme)
            throw new NotSupportedException($"This blob storage provider instance does not support the '{blobUri.Scheme}' blob URI scheme.");

        return;
    }

    private static KeyNotFoundException CreateBlobNotFoundException(BlobReference blobReference)
    {
        KeyNotFoundException exception = new($"This blob storage provider instance does not contain the '{blobReference}' blob.");
        return exception;
    }

    private void ThrowIfBlobBucketIsNotConfigured(string blobBucket)
    {
        if (!HasBlobBucket(blobBucket))
            throw new NotSupportedException($"This blob storage provider instance has no '{blobBucket}' blob bucket configured.");

        return;
    }

    private void ThrowIfBlobOperationIsNotSupported(BlobOperation blobOperation)
    {
        if (!SupportsBlobOperation(blobOperation))
            throw new NotSupportedException($"This blob storage provider instance does not support the '{blobOperation}' blob operation.");

        return;
    }

    private void ThrowIfBlobOperationIsNotAllowed(BlobOperation blobOperation)
    {
        if (!AllowsBlobOperation(blobOperation))
            throw new InvalidOperationException($"This blob storage provider instance does not allow the '{blobOperation}' blob operation.");

        return;
    }

    private class BlobInfo
    {
        public required string Bucket { get; set; }

        public required BlobReference Reference { get; set; }

        public required BlobMetadata Metadata { get; set; }

        public DateTimeOffset? ExpiresOn { get; set; }

        public required MemoryStream MemoryStream { get; set; }
    }
}
