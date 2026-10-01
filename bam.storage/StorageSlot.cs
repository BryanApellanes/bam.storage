namespace Bam.Storage;

/// <summary>
/// Abstract base class for storage slots that provides file-system-based data reading and a static factory
/// for creating segmented-path slots from hash hex strings.
/// </summary>
public abstract class StorageSlot : IStorageSlot
{
    /// <summary>
    /// Initializes a new instance of <see cref="StorageSlot"/> with the default relative path "dat".
    /// </summary>
    public StorageSlot(): this(SegmentedPath.DataFileName)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StorageSlot"/> with the specified relative path
    /// and the working directory as the storage holder.
    /// </summary>
    /// <param name="relativePath">The relative path (name) of this slot.</param>
    /// <exception cref="ArgumentException">The path is rooted, empty, contains an unsafe segment, or resolves outside the holder.</exception>
    public StorageSlot(string relativePath) : this(DirectoryStorageHolder.WorkingDirectoryHolder, relativePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StorageSlot"/> with the specified storage holder and relative path.
    /// </summary>
    /// <remarks>
    /// The path is checked twice: <see cref="StoragePathGuard.EnsureRelativePath"/> judges its text, then
    /// <see cref="StoragePathGuard.EnsureContained"/> resolves it against the holder and refuses a location outside it.
    /// The second check needs the holder's path, so it is skipped for a holder that has none yet.
    /// </remarks>
    /// <param name="storageHolder">The storage holder that contains this slot.</param>
    /// <param name="relativePath">The relative path (name) of this slot within the holder.</param>
    /// <exception cref="ArgumentException">The path is rooted, empty, contains an unsafe segment, or resolves outside the holder.</exception>
    public StorageSlot(IStorageHolder storageHolder, string relativePath)
    {
        this.Name = StoragePathGuard.EnsureRelativePath(relativePath, nameof(relativePath));
        string? holderFullName = storageHolder?.FullName;
        if (!string.IsNullOrEmpty(holderFullName))
        {
            StoragePathGuard.EnsureContained(holderFullName, relativePath, nameof(relativePath));
        }

        this.StorageHolder = storageHolder;
    }

    /// <summary>
    /// Gets the full path of this slot, combining the storage holder's path with the slot name.
    /// </summary>
    public virtual string? FullName => Path.Combine(StorageHolder!.FullName!, Name);

    /// <summary>
    /// Gets the storage holder that contains this slot.
    /// </summary>
    public IStorageHolder? StorageHolder { get; protected set; }

    /// <summary>
    /// Gets the relative path (name) of this slot within its storage holder.
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// Gets or sets the cached raw data for this slot.
    /// </summary>
    protected IRawData RawData { get; set; } = null!;

    /// <summary>
    /// Reads and returns the raw data from the file at this slot's full path. Caches the result for subsequent calls.
    /// </summary>
    /// <returns>The raw data stored at this slot's file path.</returns>
    /// <exception cref="ArgumentException">No file exists at this slot's full path.</exception>
    public virtual IRawData? GetData()
    {
        if (RawData != null)
        {
            return RawData;
        }

        string filePath = FullName!;
        if (File.Exists(filePath))
        {
            RawData = new RawData(File.ReadAllBytes(filePath));
        }
        else
        {
            throw new ArgumentException($"slot not found {FullName}");
        }

        return RawData;
    }


    /// <summary>
    /// Writes the specified raw data to this storage slot.
    /// </summary>
    /// <param name="rawData">The raw data to store.</param>
    public abstract void SetData(IRawData rawData);

    /// <summary>
    /// Creates a storage slot with a segmented directory path derived from splitting the hash hex string
    /// into two-character segments. For example, hash "abcdef" produces path "ab/cd/ef/dat".
    /// </summary>
    /// <param name="rootHolder">The root storage holder for the segmented path.</param>
    /// <param name="hashHexString">The hex-encoded hash string to split into path segments.</param>
    /// <returns>A file-system storage slot at the segmented path, inside <paramref name="rootHolder"/>.</returns>
    /// <exception cref="ArgumentException">The identifier produces a segment that <see cref="StoragePathGuard.EnsureSafeSegment"/> refuses, or a path that resolves outside <paramref name="rootHolder"/>.</exception>
    public static IStorageSlot GetSegmentedPathStorageSlot(IStorageHolder rootHolder, string hashHexString)
    {
        return new FsStorageSlot(rootHolder, SegmentedPath.FromHashHexString(hashHexString));
    }
}
