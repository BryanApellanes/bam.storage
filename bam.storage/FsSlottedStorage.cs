namespace Bam.Storage;

/// <summary>
/// File-system-based slotted storage that stores data as files within a root directory.
/// Supports content-addressable storage using hash-based segmented paths.
/// </summary>
public class FsSlottedStorage : SlottedStorage
{
    /// <summary>
    /// Implicitly converts an <see cref="FsSlottedStorage"/> to a <see cref="DirectoryInfo"/> representing its root directory.
    /// </summary>
    /// <param name="slottedStorage">The slotted storage to convert.</param>
    public static implicit operator DirectoryInfo(FsSlottedStorage slottedStorage)
    {
        return slottedStorage.Directory;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsSlottedStorage"/> using a "storage" subdirectory under the current working directory.
    /// </summary>
    public FsSlottedStorage()
    {
        this.Directory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "storage"));
        this.RootHolder = new FsStorageHolder(this.Directory);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsSlottedStorage"/> using the specified directory path as the root.
    /// </summary>
    /// <param name="path">The root directory path for storage.</param>
    public FsSlottedStorage(string path)
    {
        this.Directory = new DirectoryInfo(path);
        this.RootHolder = new FsStorageHolder(this.Directory);
    }

    /// <summary>
    /// Gets the root directory where storage files are located.
    /// </summary>
    public DirectoryInfo Directory { get; }

    /// <summary>
    /// Gets the root storage holder for this slotted storage.
    /// </summary>
    public override IStorageHolder RootHolder { get; }

    /// <summary>
    /// Gets or sets the current default storage slot.
    /// </summary>
    public override IStorageSlot CurrentSlot { get; set; }

    /// <summary>
    /// Gets the current slot if set, otherwise returns a default slot named "dat".
    /// </summary>
    /// <returns>The current or default storage slot.</returns>
    public override IStorageSlot GetSlot()
    {
        return CurrentSlot ?? GetSlot("dat");
    }

    /// <summary>
    /// Gets a file-system storage slot at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path within the root directory.</param>
    /// <returns>A file-system storage slot at the specified path.</returns>
    public override IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(RootHolder, relativePath);
    }

    /// <summary>
    /// Saves raw data to the current slot if set, or to a hash-derived segmented path slot.
    /// </summary>
    /// <param name="data">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public override IStorageSlot Save(IRawData data)
    {
        return Save(this.CurrentSlot ?? this.GetHashHexStringStorageSlot(data.HashHexString), data);
    }

    /// <summary>
    /// Saves a byte array by wrapping it as <see cref="RawData"/> and saving it.
    /// </summary>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public override IStorageSlot Save(byte[] data)
    {
        RawData rawData = new RawData(data);
        return Save(rawData);
    }

    /// <summary>
    /// Saves a byte array to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to write to.</param>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public override IStorageSlot Save(IStorageSlot slot, byte[] data)
    {
        return this.Save(slot, new RawData(data));
    }

    /// <summary>
    /// Saves a byte array at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path for the storage slot.</param>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public override IStorageSlot Save(string relativePath, byte[] data)
    {
        return Save(relativePath, new RawData(data));
    }

    /// <summary>
    /// Saves raw data at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path for the storage slot.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public override IStorageSlot Save(string relativePath, IRawData rawData)
    {
        return Save(this.GetSlot(relativePath), rawData);
    }

    /// <summary>
    /// Saves raw data to the specified storage slot. Validates that the slot belongs to this storage's root directory.
    /// </summary>
    /// <param name="slot">The storage slot to write to.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public override IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        Args.ThrowIfNull(RootHolder, nameof(RootHolder));
        Args.ThrowIf(!slot.StorageHolder.FullName.StartsWith(RootHolder.FullName, StringComparison.InvariantCultureIgnoreCase), $"{nameof(FsSlottedStorage)}:: slot is in {slot.StorageHolder.FullName} not in storage root {RootHolder.FullName}");

        slot.SetData(rawData);
        return slot;
    }

    /// <summary>
    /// Loads raw data from a segmented-path slot derived from the specified hash hex string.
    /// </summary>
    /// <param name="hashHexString">The hex-encoded hash string identifying the data.</param>
    /// <returns>The raw data identified by the hash.</returns>
    public override IRawData LoadHashHexString(string hashHexString)
    {
        return LoadSlot(GetHashHexStringStorageSlot(hashHexString));
    }

    /// <summary>
    /// Loads raw data from the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot.</param>
    /// <returns>The raw data loaded from the slot.</returns>
    public override IRawData Load(string relativePath)
    {
        return LoadSlot(GetSlot(relativePath));
    }

    /// <summary>
    /// Loads raw data from the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to load data from.</param>
    /// <returns>The raw data loaded from the slot.</returns>
    public override IRawData LoadSlot(IStorageSlot slot)
    {
        return slot.GetData();
    }

    /// <summary>
    /// Gets a storage slot with a segmented directory path derived from the specified hash hex string.
    /// </summary>
    /// <param name="hashHexString">The hex-encoded hash string to split into path segments.</param>
    /// <returns>A storage slot at the segmented path.</returns>
    public virtual IStorageSlot GetHashHexStringStorageSlot(string hashHexString)
    {
        return FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hashHexString);
    }
}