using System;

namespace AndreyTalanin0x00.Integrations.Blobs.Configuration;

public class InMemoryBlobStorageProviderConfiguration
{
    public static string SectionName { get; } = "InMemoryBlobStorageProvider";

    public TimeSpan ExpiredBlobCleanUpPeriod { get; set; }
}
