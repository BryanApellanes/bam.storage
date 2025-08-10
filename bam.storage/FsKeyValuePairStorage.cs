namespace Bam.Storage;

public class FsKeyValuePairStorage : IKeyValuePairStorage
{
    public FsKeyValuePairStorage()
    {
        this.FsStorage = new FsStorage();
    }

    public FsKeyValuePairStorage(string rootPath)
    {
        this.FsStorage = new FsStorage(rootPath);
    }

    public FsKeyValuePairStorage(FsStorage storage)
    {
        this.FsStorage = storage;
    }
    
    protected FsStorage FsStorage { get; set; }
    
    public virtual IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair)
    {
        Args.ThrowIfNull(keyValuePair, nameof(keyValuePair));
        try
        {
            IStorageSlot slot = FsStorage.GetHashHexStringStorageSlot(keyValuePair.Key);
            FsStorage.Save(slot, keyValuePair.Value);
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
        IStorageSlot slot = FsStorage.GetHashHexStringStorageSlot(key);
        IRawData rawData = slot.GetData();
        return new KeyValuePair()
        {
            Key = key,
            Value = rawData.Value,
        };
    }
}