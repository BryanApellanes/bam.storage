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
    public StorageSlot(): this("dat")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StorageSlot"/> with the specified relative path
    /// and the working directory as the storage holder.
    /// </summary>
    /// <param name="relativePath">The relative path (name) of this slot.</param>
    public StorageSlot(string relativePath)
    {
        this.StorageHolder = DirectoryStorageHolder.WorkingDirectoryHolder;
        this.Name = relativePath;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StorageSlot"/> with the specified storage holder and relative path.
    /// </summary>
    /// <param name="storageHolder">The storage holder that contains this slot.</param>
    /// <param name="relativePath">The relative path (name) of this slot within the holder.</param>
    public StorageSlot(IStorageHolder storageHolder, string relativePath) : this(relativePath)
    {
        this.StorageHolder = storageHolder;
    }

    /// <summary>
    /// Gets the full path of this slot, combining the storage holder's path with the slot name.
    /// </summary>
    public virtual string? FullName => Path.Combine(StorageHolder.FullName, Name);

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
    protected IRawData RawData { get; set; }

    /// <summary>
    /// Reads and returns the raw data from the file at this slot's full path. Caches the result for subsequent calls.
    /// Throws <see cref="ArgumentException"/> if the file does not exist.
    /// </summary>
    /// <returns>The raw data stored at this slot's file path.</returns>
    public virtual IRawData? GetData()
    {
        if (RawData != null)
        {
            return RawData;
        }

        string filePath = FullName;
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
    /// <returns>A file-system storage slot at the segmented path.</returns>
    public static IStorageSlot GetSegmentedPathStorageSlot(IStorageHolder rootHolder, string hashHexString)
    {
        Args.ThrowIfNullOrEmpty(hashHexString, nameof(hashHexString));

        List<string> parts = new List<string>();
        parts.AddRange(hashHexString.Split(2));
        parts.Add("dat");
        return new FsStorageSlot(rootHolder, Path.Combine(parts.ToArray()));
    }
}