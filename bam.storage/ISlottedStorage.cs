namespace Bam.Storage;

/// <summary>
/// Extends <see cref="IRawStorage"/> with slot-based addressing, allowing data to be saved and loaded
/// by relative path, storage slot, or raw byte array.
/// </summary>
public interface ISlottedStorage : IRawStorage
{
    /// <summary>
    /// Gets the root storage holder that contains all slots managed by this storage.
    /// </summary>
    IStorageHolder RootHolder { get; }

    /// <summary>
    /// Gets or sets the current default storage slot used when no slot is explicitly specified.
    /// </summary>
    IStorageSlot CurrentSlot { get; set; }

    /// <summary>
    /// Gets a storage slot at the specified relative path within the root holder.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot.</param>
    /// <returns>The storage slot at the specified path.</returns>
    IStorageSlot GetSlot(string relativePath);

    /// <summary>
    /// Saves raw data to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to save data to.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    IStorageSlot Save(IStorageSlot slot, IRawData rawData);

    /// <summary>
    /// Saves raw data at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path for the storage slot.</param>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    IStorageSlot Save(string relativePath, IRawData rawData);

    /// <summary>
    /// Saves a byte array to the current or default storage slot.
    /// </summary>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    IStorageSlot Save(byte[] data);

    /// <summary>
    /// Saves a byte array to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to save data to.</param>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    IStorageSlot Save(IStorageSlot slot, byte[] data);

    /// <summary>
    /// Saves a byte array at the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path for the storage slot.</param>
    /// <param name="data">The byte array to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    IStorageSlot Save(string relativePath, byte[] data);

    /// <summary>
    /// Loads raw data from the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to load data from.</param>
    /// <returns>The raw data loaded from the slot.</returns>
    IRawData LoadSlot(IStorageSlot slot);

    /// <summary>
    /// Loads raw data from the specified relative path.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot to load from.</param>
    /// <returns>The raw data loaded from the slot.</returns>
    IRawData Load(string relativePath);
}