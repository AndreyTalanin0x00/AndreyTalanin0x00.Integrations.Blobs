using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

// Disable the IDE0028 (Simplify collection initialization) notification to preserve explicit collection implementation types.
#pragma warning disable IDE0028

namespace AndreyTalanin0x00.Integrations.Blobs.Options;

internal class BlobManagerOptions
{
    public ICollection<Action<IServiceCollection>> AddBlobStorageProviderServiceCollectionVisitors { get; set; } = new List<Action<IServiceCollection>>();
}
