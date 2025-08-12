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
    /// <param name="rawData"></param>
    /// <returns></returns>
    IStorageSlot Save(IRawData rawData);
    
    /// <summary>
    /// Load the data identified by the specified hash in hex format.
    /// </summary>
    /// <param name="hashHexString"></param>
    /// <returns></returns>
    IRawData LoadHashHexString(string hashHexString);
}