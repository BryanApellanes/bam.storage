using Bam;

namespace Bam.Storage;

public class FsStorageSlot : IStorageSlot
{
    public FsStorageSlot(): this("dat")
    {
    }
    
    public FsStorageSlot(IStorageHolder storageHolder, string relativePath):this(relativePath)
    {
        this.StorageHolder = storageHolder;
    }
    
    public FsStorageSlot(string relativePath)
    {
        this.StorageHolder = DirectoryStorageHolder.WorkingDirectoryHolder;
    }

    public string? FullName => Path.Combine(StorageHolder.FullName, Name);

    public IStorageHolder? StorageHolder { get; protected set; }
    public string Name { get; }

    private IRawData _data;
    public virtual IRawData? GetData()
    {
        if (_data != null)
        {
            return _data;
        }

        string filePath = FullName;
        if (File.Exists(filePath))
        {
            _data = new RawData(File.ReadAllBytes(filePath));
        }

        return _data;
    }

    public virtual void SetData(IRawData rawData)
    {
        this._data = rawData;
    }
}