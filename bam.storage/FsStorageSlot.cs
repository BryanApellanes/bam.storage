using Bam.Net;

namespace Bam.Storage;

public class FsStorageSlot : IStorageSlot
{
    public FsStorageSlot(): this("dat")
    {
    }
    
    public FsStorageSlot(IStorageContainer storageContainer, string relativePath):this(relativePath)
    {
        this.StorageContainer = storageContainer;
    }
    
    public FsStorageSlot(string relativePath)
    {
        this.StorageContainer = DirectoryStorageContainer.WorkingDirectoryContainer;
        this.RelativePath = relativePath;
    }

    public string? FullName => StorageContainer != null ? Path.Combine(StorageContainer.FullName, RelativePath) : RelativePath;

    public IStorageContainer? StorageContainer { get; }
    public string RelativePath { get; }
    public string Name { get; }

    private IRawData _data;
    public virtual IRawData? GetData()
    {
        if (_data != null)
        {
            return _data;
        }
        
        string filePath = Path.Combine(StorageContainer.FullName, RelativePath);
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