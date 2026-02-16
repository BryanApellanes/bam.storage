namespace Bam.Storage;

/// <summary>
/// When implemented in a derived class, provides storage for data
/// addressable by the hash of the data in hex format.
/// </summary>
public interface IRawStorage
{
    /// <summary>
    /// Save the specified data.
    /// </summary>
    /// <param name="rawData">The raw data to save.</param>
    /// <returns>The storage slot where the data was saved.</returns>
    IStorageSlot Save(IRawData rawData);

    /// <summary>
    /// Load the data identified by the specified hash in hex format.
    /// </summary>
    /// <param name="hashHexString">The hex-encoded hash string identifying the data.</param>
    /// <returns>The raw data identified by the specified hash.</returns>
    IRawData LoadHashHexString(string hashHexString);
}