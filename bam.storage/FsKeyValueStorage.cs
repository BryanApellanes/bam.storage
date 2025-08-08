namespace Bam.Storage;

public class FsKeyValueStorage : IKeyValueStorage
{
    public FsKeyValueStorage()
    {
        this.FsStorage = new FsStorage();
    }

    public FsKeyValueStorage(string rootPath)
    {
        this.FsStorage = new FsStorage(rootPath);
    }
    
    protected FsStorage FsStorage { get; set; }
    
    public IKeyValueSaveResult Save(IKeyValue keyValue)
    {
        Args.ThrowIfNull(keyValue, nameof(keyValue));
        try
        {
            IStorageSlot slot = FsStorage.GetHashHexStringStorageSlot(keyValue.Key);
            FsStorage.Save(slot, keyValue.Value);
            return new KeyValueSaveResult()
            {
                Success = true,
                Key = keyValue.Key,
            };
        }
        catch (Exception ex)
        {
            return new KeyValueSaveResult()
            {
                Key = keyValue.Key,
                Success = false,
                ErrorMessage = ex.Message,
            };
        }
    }

    public IKeyValue Get(string key)
    {
        IStorageSlot slot = FsStorage.GetHashHexStringStorageSlot(key);
        IRawData rawData = slot.GetData();
        return new KeyValue()
        {
            Key = key,
            Value = rawData.Value,
        };
    }
}