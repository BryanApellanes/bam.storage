namespace Bam.Storage;

/// <summary>
/// Abstract base class for slot-based storage implementations, providing the contract for
/// saving and loading data by slot, relative path, hash hex string, or raw byte array.
/// </summary>
public abstract class SlottedStorage : ISlottedStorage
{
    /// <summary>
    /// Gets the root storage holder that contains all slots managed by this storage.
    /// </summary>
    public abstract IStorageHolder RootHolder { get; }

    /// <summary>
    /// Gets or sets the current default storage slot used when no slot is explicitly specified.
    /// </summary>
    public abstract IStorageSlot CurrentSlot { get; set; }

    /// <summary>
    /// Gets the current default storage slot, or a new slot if none is set.
    /// </summary>
    /// <returns>The current or default storage slot.</returns>
    public abstract IStorageSlot GetSlot();

    /// <summary>
    /// Gets a storage slot at the specified relative path within the root holder.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot.</param>
    /// <returns>The storage slot at the specified path.</returns>
    public abstract IStorageSlot GetSlot(string relativePath);

    /// <summary>
    /// Saves raw data to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to save data to.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public abstract IStorageSlot Save(IStorageSlot slot, IRawData rawData);

    /// <summary>
    /// Saves raw data to the current or default storage slot.
    /// </summary>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public abstract IStorageSlot Save(IRawData rawData);

    /// <summary>
    /// Saves raw data at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path for the storage slot.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public abstract IStorageSlot Save(string relativePath, IRawData rawData);

    /// <summary>
    /// Saves a byte array to the current or default storage slot.
    /// </summary>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public abstract IStorageSlot Save(byte[] data);

    /// <summary>
    /// Saves a byte array to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to save data to.</param>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public abstract IStorageSlot Save(IStorageSlot slot, byte[] data);

    /// <summary>
    /// Saves a byte array at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path for the storage slot.</param>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    public abstract IStorageSlot Save(string relativePath, byte[] data);

    /// <summary>
    /// Loads raw data from the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to load data from.</param>
    /// <returns>The raw data loaded from the slot.</returns>
    public abstract IRawData LoadSlot(IStorageSlot slot);

    /// <summary>
    /// Loads raw data identified by the specified hash hex string.
    /// </summary>
    /// <param name="hashHexString">The hex-encoded hash string identifying the data.</param>
    /// <returns>The raw data identified by the hash.</returns>
    public abstract IRawData LoadHashHexString(string hashHexString);

    /// <summary>
    /// Loads raw data from the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot.</param>
    /// <returns>The raw data loaded from the slot.</returns>
    public abstract IRawData Load(string relativePath);


    /// <summary>
    /// Reads all bytes from the file at the specified path.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
    /// <returns>The byte contents of the file.</returns>
    protected virtual byte[] ReadBytes(string path)
    {
        return File.ReadAllBytes(path);
    }
}