namespace Bam.Storage;

/// <summary>
/// A file-system-based storage holder backed by a directory. Supports implicit conversion to and from
/// strings and <see cref="DirectoryInfo"/>.
/// </summary>
public class FsStorageHolder : IStorageHolder
{
    /// <summary>
    /// Implicitly converts an <see cref="FsStorageHolder"/> to its full directory path string.
    /// </summary>
    /// <param name="fsStorageHolder">The storage holder to convert.</param>
    public static implicit operator string?(FsStorageHolder fsStorageHolder)
    {
        return fsStorageHolder.FullName;
    }

    /// <summary>
    /// Implicitly converts a directory path string to an <see cref="FsStorageHolder"/>.
    /// </summary>
    /// <param name="value">The directory path.</param>
    public static implicit operator FsStorageHolder(string value)
    {
        return new FsStorageHolder(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="FsStorageHolder"/> to a <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="fsStorageHolder">The storage holder to convert.</param>
    public static implicit operator DirectoryInfo(FsStorageHolder fsStorageHolder)
    {
        return fsStorageHolder.Directory;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsStorageHolder"/> for the specified directory path.
    /// </summary>
    /// <param name="path">The directory path.</param>
    public FsStorageHolder(string path) : this(new DirectoryInfo(path))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsStorageHolder"/> for the specified directory.
    /// </summary>
    /// <param name="directory">The directory that this holder represents.</param>
    public FsStorageHolder(DirectoryInfo directory)
    {
        this.Directory = directory;
    }

    /// <summary>
    /// Gets the full path of the directory this holder represents.
    /// </summary>
    public virtual string? FullName
    {
        get => Directory?.FullName;
        private init => Directory = new DirectoryInfo(value);
    }

    /// <summary>
    /// Gets a storage slot at the specified relative path within this directory.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot.</param>
    /// <returns>A file-system-based storage slot.</returns>
    public IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(this, relativePath);
    }

    /// <summary>
    /// Combines this holder's full path with the specified path parts into a single path.
    /// </summary>
    /// <param name="pathParts">The path segments to append to this holder's path.</param>
    /// <returns>The combined path string.</returns>
    public string Combine(params string[] pathParts)
    {
        List<string?> parts = new List<string?> { FullName };
        parts.AddRange(pathParts);
        return Path.Combine(parts.ToArray());
    }

    /// <summary>
    /// Gets the underlying <see cref="DirectoryInfo"/> for this storage holder.
    /// </summary>
    public DirectoryInfo Directory
    {
        get;
        private init;
    }
}