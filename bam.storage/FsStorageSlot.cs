using Bam.Net;

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
        this.RelativePath = relativePath;
    }

    public string? FullName => StorageHolder != null ? Path.Combine(StorageHolder.FullName, RelativePath) : RelativePath;

    public IStorageHolder? StorageHolder { get; }
    public string RelativePath { get; }
    public string Name { get; }

    private IRawData _data;
    public virtual IRawData? GetData()
    {
        if (_data != null)
        {
            return _data;
        }
        
        string filePath = Path.Combine(StorageHolder.FullName, RelativePath);
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