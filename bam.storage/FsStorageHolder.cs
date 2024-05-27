using Bam;

namespace Bam.Storage;

public class FsStorageHolder : IStorageHolder
{
    public static implicit operator string?(FsStorageHolder fsStorageHolder)
    {
        return fsStorageHolder.FullName;
    }

    public static implicit operator FsStorageHolder(string value)
    {
        return new FsStorageHolder(value);
    }

    public static implicit operator DirectoryInfo(FsStorageHolder fsStorageHolder)
    {
        return fsStorageHolder.Directory;
    }
    
    public FsStorageHolder(string path) : this(new DirectoryInfo(path))
    {
    }

    public FsStorageHolder(DirectoryInfo directory)
    {
        this.Directory = directory;
    }
    
    public virtual string? FullName
    {
        get => Directory?.FullName;
        private init => Directory = new DirectoryInfo(value);
    }

    public IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(this, relativePath);
    }

    public string Combine(params string[] pathParts)
    {
        List<string?> parts = new List<string?> { FullName };
        parts.AddRange(pathParts);
        return Path.Combine(parts.ToArray());
    }

    public DirectoryInfo Directory
    {
        get;
        private init;
    }
}