using System;
using System.Diagnostics.CodeAnalysis;

namespace AndreyTalanin0x00.Integrations.Blobs;

public class BlobReference
{
    public required Guid Id { get; set; }

    public required Uri Uri { get; set; }

    public BlobReference()
    {
    }

    [SetsRequiredMembers]
    public BlobReference(Guid id, Uri uri)
    {
        Id = id;
        Uri = uri;
    }

    public void Deconstruct(out Guid id, out Uri uri)
    {
        (id, uri) = (Id, Uri);
    }

    public override bool Equals(object? other)
    {
        if (other == this)
            return true;

        if (other is not BlobReference otherBlobReference)
            return false;

        if (otherBlobReference.Id != Id)
            return false;

        bool result = otherBlobReference.Uri.Equals(Uri);

        return result;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Uri);
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}
