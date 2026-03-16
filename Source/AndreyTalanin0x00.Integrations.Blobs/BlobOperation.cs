namespace AndreyTalanin0x00.Integrations.Blobs;

public enum BlobOperation
{
    Unknown,

    Upload,

    Download,

    GetMetadata,

    GetExpirationTime,

    UpdateMetadata,

    UpdateExpirationTime,

    Delete,
}
