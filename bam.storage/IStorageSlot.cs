namespace Bam.Storage;

/// <summary>
/// Represents an individual addressable storage location within a storage holder, capable of reading and writing raw data.
/// </summary>
public interface IStorageSlot : IStorageIdentifier
{
    /// <summary>
    /// Gets the storage holder that contains this slot.
    /// </summary>
    IStorageHolder? StorageHolder { get; }

    /// <summary>
    /// Gets the name (relative path) of this slot within its storage holder.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Reads and returns the raw data stored in this slot.
    /// </summary>
    /// <returns>The raw data stored in this slot, or null if no data is present.</returns>
    IRawData? GetData();

    /// <summary>
    /// Writes the specified raw data to this slot.
    /// </summary>
    /// <param name="rawData">The raw data to store in this slot.</param>
    void SetData(IRawData rawData);
}