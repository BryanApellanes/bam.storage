namespace Bam.Storage;

public class FsKeyValuePairStorage : IKeyValuePairStorage
{
    public FsKeyValuePairStorage()
    {
        this.FsSlottedStorage = new FsSlottedStorage();
    }

    public FsKeyValuePairStorage(string rootPath)
    {
        this.FsSlottedStorage = new FsSlottedStorage(rootPath);
    }

    public FsKeyValuePairStorage(FsSlottedStorage slottedStorage)
    {
        this.FsSlottedStorage = slottedStorage;
    }
    
    protected FsSlottedStorage FsSlottedStorage { get; set; }

    public IKeyValuePairSaveResult Save(string key, string value)
    {
        return Save(new KeyValuePair(key, value));
    }

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

    public virtual IKeyValuePair Get(string key)
    {
        IStorageSlot slot = FsSlottedStorage.GetHashHexStringStorageSlot(key);
        IRawData rawData = slot.GetData();
        return new KeyValuePair()
        {
            Key = key,
            Value = rawData.Value,
        };
    }
}