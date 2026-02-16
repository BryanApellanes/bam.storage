namespace Bam.Storage;

/// <summary>
/// A directory-based storage holder designated as the root of a storage hierarchy.
/// </summary>
public class RootStorageHolder : DirectoryStorageHolder, IRootStorageHolder
{
    /// <summary>
    /// Initializes a new instance of <see cref="RootStorageHolder"/> for the specified directory path.
    /// </summary>
    /// <param name="path">The root directory path.</param>
    public RootStorageHolder(string path) : base(path)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RootStorageHolder"/> for the specified directory.
    /// </summary>
    /// <param name="directory">The root directory.</param>
    public RootStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }
}