namespace Bam.Storage;

/// <summary>
/// File-system-based raw storage that saves and loads data using content-addressable segmented paths
/// derived from the data's hash hex string.
/// </summary>
public class FsRawStorage : IRawStorage
{
    /// <summary>
    /// Initializes a new instance of <see cref="FsRawStorage"/> using the working directory data holder as the root.
    /// </summary>
    public FsRawStorage()
    {
        this.RootHolder = DirectoryStorageHolder.WorkingDirectoryHolder;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsRawStorage"/> using the specified storage holder as the root.
    /// </summary>
    /// <param name="rootHolder">The root storage holder.</param>
    public FsRawStorage(IStorageHolder rootHolder)
    {
        this.RootHolder = rootHolder;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsRawStorage"/> using the specified directory path as the root.
    /// </summary>
    /// <param name="path">The root directory path.</param>
    public FsRawStorage(string path)
    {
        this.RootHolder = new FsStorageHolder(path);
    }

    /// <summary>
    /// Gets the current slot if set, otherwise returns a default slot named "dat".
    /// </summary>
    /// <returns>The current or default storage slot.</returns>
    public virtual IStorageSlot GetSlot()
    {
        return CurrentSlot ?? GetSlot("dat");
    }

    /// <summary>
    /// Gets a file-system storage slot at the specified relative path within the root holder.
    /// </summary>
    /// <param name="relativePath">The relative path within the root directory.</param>
    /// <returns>A file-system storage slot.</returns>
    public virtual IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(RootHolder, relativePath);
    }

    /// <summary>
    /// Gets the root storage holder for this raw storage.
    /// </summary>
    public IStorageHolder RootHolder { get; }

    /// <summary>
    /// Gets or sets the current default storage slot.
    /// </summary>
    public IStorageSlot CurrentSlot { get; set; } = null!;

    /// <summary>
    /// Saves raw data to the current slot if set, or to a hash-derived segmented path slot.
    /// </summary>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public virtual IStorageSlot Save(IRawData rawData)
    {
        return Save(this.CurrentSlot ?? FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, rawData.HashHexString), rawData);
    }

    /// <summary>
    /// Loads raw data from a segmented-path slot derived from the specified hash hex string.
    /// </summary>
    /// <param name="hashHexString">The hex-encoded hash string identifying the data.</param>
    /// <returns>The raw data identified by the hash.</returns>
    public virtual IRawData LoadHashHexString(string hashHexString)
    {
        return FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hashHexString).GetData()!;
    }

    /// <summary>
    /// Saves raw data to the specified slot. Validates that the slot belongs to this storage's root directory.
    /// </summary>
    /// <param name="slot">The storage slot to write to.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    protected virtual IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        Args.ThrowIfNull(slot, nameof(slot));
        Args.ThrowIfNull(RootHolder, nameof(RootHolder));
        Args.ThrowIf(!slot.StorageHolder!.FullName!.StartsWith(RootHolder.FullName!, StringComparison.InvariantCultureIgnoreCase), $"{nameof(FsRawStorage)}:: slot is in {slot.StorageHolder.FullName} not in storage root {RootHolder.FullName}");

        slot.SetData(rawData);
        return slot;
    }
}