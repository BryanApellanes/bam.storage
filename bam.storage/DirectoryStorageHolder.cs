namespace Bam.Storage;

/// <summary>
/// A directory-based storage holder that provides singleton access to common storage directories
/// (working directory data and user profile data).
/// </summary>
public class DirectoryStorageHolder : FsStorageHolder, IStorageHolder
{
    /// <summary>
    /// Initializes a new instance of <see cref="DirectoryStorageHolder"/> using the default profile data path.
    /// </summary>
    public DirectoryStorageHolder() : base(BamProfile.DataPath)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DirectoryStorageHolder"/> for the specified directory path.
    /// </summary>
    /// <param name="path">The directory path.</param>
    public DirectoryStorageHolder(string path) : base(path)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DirectoryStorageHolder"/> for the specified directory.
    /// </summary>
    /// <param name="directory">The directory that this holder represents.</param>
    public DirectoryStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }

    private static DirectoryStorageHolder _workingDirectoryHolder;
    private static readonly object _workingDirectoryHolderLock = new object();

    /// <summary>
    /// Gets the thread-safe singleton storage holder for the working directory's data folder.
    /// </summary>
    public static DirectoryStorageHolder WorkingDirectoryHolder
    {
        get
        {
            return _workingDirectoryHolderLock.DoubleCheckLock(ref _workingDirectoryHolder,
                () => new DirectoryStorageHolder(BamDir.Data));
        }
    }

    private static DirectoryStorageHolder _profileDirectoryHolder;
    private static readonly object _profileDirectoryContainerLock = new object();

    /// <summary>
    /// Gets the thread-safe singleton storage holder for the user profile data directory.
    /// </summary>
    public static DirectoryStorageHolder ProfileDirectoryHolder
    {
        get
        {
            return _profileDirectoryContainerLock.DoubleCheckLock(ref _profileDirectoryHolder,
                () => new DirectoryStorageHolder(BamProfile.DataPath));
        }
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
}