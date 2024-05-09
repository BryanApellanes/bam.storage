using Bam.Net;

namespace Bam.Storage;

public class FsStorageContainer : IStorageContainer
{
    public static implicit operator string?(FsStorageContainer fsStorageContainer)
    {
        return fsStorageContainer.FullName;
    }

    public static implicit operator FsStorageContainer(string value)
    {
        return new FsStorageContainer(value);
    }

    public static implicit operator DirectoryInfo(FsStorageContainer fsStorageContainer)
    {
        return fsStorageContainer.Directory;
    }
    
    public FsStorageContainer(string path) : this(new DirectoryInfo(path))
    {
    }

    public FsStorageContainer(DirectoryInfo directory)
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