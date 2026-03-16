using System;
using System.Collections.Generic;

using AndreyTalanin0x00.Integrations.Blobs.Helpers;
using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

// Disable the IDE0028 (Simplify collection initialization) notification to preserve explicit collection implementation types.
#pragma warning disable IDE0028

namespace AndreyTalanin0x00.Integrations.Blobs.Options;

public class BlobStorageProviderOptions<TBlobStorageProvider>
    where TBlobStorageProvider : class, IBlobStorageProvider
{
    public virtual ICollection<string> BlobBuckets { get; set; } = new List<string>();

    public virtual ICollection<BlobOperation> AllowedBlobOperations { get; set; } = new List<BlobOperation>();

    public BlobStorageProviderOptions<TBlobStorageProvider> GetImmutableCopy()
    {
        ImmutableBlobStorageProviderOptions immutableBlobStorageProviderOptions =
            new(this);

        return immutableBlobStorageProviderOptions;
    }

    private class ImmutableBlobStorageProviderOptions : BlobStorageProviderOptions<TBlobStorageProvider>
    {
        public ImmutableBlobStorageProviderOptions(BlobStorageProviderOptions<TBlobStorageProvider> blobStorageProviderOptions)
        {
            base.BlobBuckets = ReadOnlyCollectionHelpers.CreateReadOnlyCollection(blobStorageProviderOptions.BlobBuckets);
            base.AllowedBlobOperations = ReadOnlyCollectionHelpers.CreateReadOnlyCollection(blobStorageProviderOptions.AllowedBlobOperations);
        }

        public override ICollection<string> BlobBuckets
        {
            get => base.BlobBuckets;
            set => throw new NotSupportedException($"You can not modify this {nameof(BlobStorageProviderOptions<TBlobStorageProvider>)} instance once it has been created.");
        }

        public override ICollection<BlobOperation> AllowedBlobOperations
        {
            get => base.AllowedBlobOperations;
            set => throw new NotSupportedException($"You can not modify this {nameof(BlobStorageProviderOptions<TBlobStorageProvider>)} instance once it has been created.");
        }
    }
}
