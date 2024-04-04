namespace Bam.Storage;

public class RawDataFs : IRawDataFs
{
    public RawDataFs(FsStorageIdentifier identifier)
    {
        this.FsStorageIdentifier = identifier;
    }
    
    protected FsStorageIdentifier FsStorageIdentifier { get; init; }

    public string Root => FsStorageIdentifier;

    public virtual string GetHashPath(string dataHash)
    {
        return FsStorageIdentifier.Combine("hash", dataHash);
    }

    public string GetIdPath(ulong hashId)
    {
        return FsStorageIdentifier.Combine("id", hashId.ToString());
    }
}