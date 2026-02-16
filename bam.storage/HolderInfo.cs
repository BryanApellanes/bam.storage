namespace Bam.Storage;

/// <summary>
/// Wraps a full slot path as an <see cref="IStorageHolder"/>, extracting the directory portion as the holder path.
/// </summary>
public class HolderInfo: IStorageHolder
{
    /// <summary>
    /// Initializes a new instance of <see cref="HolderInfo"/> using the default profile data path with a "dat" file name.
    /// </summary>
    public HolderInfo(): this(Path.Combine(DirectoryStorageHolder.ProfileDirectoryHolder, "dat"))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="HolderInfo"/> from a full slot path, using its directory as the holder path.
    /// </summary>
    /// <param name="fullSlotPath">The full path to a storage slot file.</param>
    public HolderInfo(string fullSlotPath)
    {
        this.FullSlotPath = fullSlotPath;
        this.FullName = Path.GetDirectoryName(fullSlotPath);
    }

    /// <summary>
    /// Gets the full path to the original storage slot file.
    /// </summary>
    public string FullSlotPath { get; }

    /// <summary>
    /// Gets the directory portion of the full slot path, serving as the holder's full name.
    /// </summary>
    public string? FullName { get; }

    /// <summary>
    /// Gets a storage slot at the specified relative path within this holder's directory.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot.</param>
    /// <returns>A file-system-based storage slot.</returns>
    public IStorageSlot GetSlot(string relativePath)
    {
        string fullPath = Path.Combine(FullName, relativePath);
        return new FsStorageSlot(this, relativePath);
    }
}