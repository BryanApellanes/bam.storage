namespace Bam.Storage;

public class FsKeyValuePairStorage : IKeyValuePairStorage
{
    public FsKeyValuePairStorage()
    {
        this.FsObjectStorage = new FsObjectStorage();
    }

    public FsKeyValuePairStorage(string rootPath)
    {
        this.FsObjectStorage = new FsObjectStorage(rootPath);
    }

    public FsKeyValuePairStorage(FsObjectStorage objectStorage)
    {
        this.FsObjectStorage = objectStorage;
    }
    
    protected FsObjectStorage FsObjectStorage { get; set; }
    
    public virtual IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair)
    {
        Args.ThrowIfNull(keyValuePair, nameof(keyValuePair));
        try
        {
            IStorageSlot slot = FsObjectStorage.GetHashHexStringStorageSlot(keyValuePair.Key);
            FsObjectStorage.Save(slot, keyValuePair.Value);
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
        IStorageSlot slot = FsObjectStorage.GetHashHexStringStorageSlot(key);
        IRawData rawData = slot.GetData();
        return new KeyValuePair()
        {
            Key = key,
            Value = rawData.Value,
        };
    }
}