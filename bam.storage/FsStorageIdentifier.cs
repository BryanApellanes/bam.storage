using Bam.Net;

namespace Bam.Storage;

public class FsStorageIdentifier : IStorageIdentifier
{
    public static implicit operator string?(FsStorageIdentifier fsStorageIdentifier)
    {
        return fsStorageIdentifier.Value;
    }

    public static implicit operator FsStorageIdentifier(string value)
    {
        return new FsStorageIdentifier(value);
    }

    public static implicit operator DirectoryInfo(FsStorageIdentifier fsStorageIdentifier)
    {
        return fsStorageIdentifier.Directory;
    }
    
    public FsStorageIdentifier(string path) : this(new DirectoryInfo(path))
    {
    }

    public FsStorageIdentifier(DirectoryInfo directory)
    {
        this.Directory = directory;
    }
    
    public string? Value
    {
        get => Directory?.FullName;
        private init => Directory = new DirectoryInfo(value);
    }

    public string Combine(params string[] pathParts)
    {
        List<string?> parts = new List<string?> { Value };
        parts.AddRange(pathParts);
        return Path.Combine(parts.ToArray());
    }

    public DirectoryInfo Directory
    {
        get;
        private init;
    }
}