namespace Bam.Storage;

/// <summary>
/// File-system-based key-value pair storage that uses a hash of the key to determine the
/// storage slot path. Values are stored as raw bytes in content-addressable file locations.
/// </summary>
public class FsKeyValuePairStorage : IKeyValuePairStorage
{
    /// <summary>
    /// Initializes a new instance of <see cref="FsKeyValuePairStorage"/> using the default slotted storage directory.
    /// </summary>
    public FsKeyValuePairStorage()
    {
        this.FsSlottedStorage = new FsSlottedStorage();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsKeyValuePairStorage"/> using the specified root directory path.
    /// </summary>
    /// <param name="rootPath">The root directory path for storage.</param>
    public FsKeyValuePairStorage(string rootPath)
    {
        this.FsSlottedStorage = new FsSlottedStorage(rootPath);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsKeyValuePairStorage"/> using the specified slotted storage.
    /// </summary>
    /// <param name="slottedStorage">The underlying slotted storage to use.</param>
    public FsKeyValuePairStorage(FsSlottedStorage slottedStorage)
    {
        this.FsSlottedStorage = slottedStorage;
    }

    /// <summary>
    /// Gets or sets the underlying file-system slotted storage used for persistence.
    /// </summary>
    protected FsSlottedStorage FsSlottedStorage { get; set; }

    /// <summary>
    /// Saves a key-value pair using string representations for both key and value.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The string value to store.</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    public IKeyValuePairSaveResult Save(string key, string value)
    {
        return Save(new KeyValuePair(key, value));
    }

    /// <summary>
    /// Saves the specified key-value pair to a hash-derived file path. Returns a success or failure result.
    /// </summary>
    /// <param name="keyValuePair">The key-value pair to save.</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    public virtual IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair)
    {
        Args.ThrowIfNull(keyValuePair, nameof(keyValuePair));
        try
        {
            IStorageSlot slot = FsSlottedStorage.GetHashHexStringStorageSlot(keyValuePair.Key);
            FsSlottedStorage.Save(slot, keyValuePair.Value);
            return new KeyValuePairSaveResult()
            {
                Success = true,
                Key = keyValuePair.Key,
            };
        }
        catch (Exception ex)
        {
            return new KeyValuePairSaveResult()
            {
                Key = keyValuePair.Key,
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    /// <summary>
    /// Retrieves the value associated with the specified key from the hash-derived storage slot.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The key-value pair containing the original key and its stored byte value.</returns>
    public virtual IKeyValuePair Get(string key)
    {
        IStorageSlot slot = FsSlottedStorage.GetHashHexStringStorageSlot(key);
        IRawData rawData = slot.GetData()!;
        return new KeyValuePair()
        {
            Key = key,
            Value = rawData!.Value,
        };
    }
}