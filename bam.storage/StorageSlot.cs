namespace Bam.Storage;

public abstract class StorageSlot : IStorageSlot
{
    public StorageSlot(): this("dat")
    {
    }

    public StorageSlot(string relativePath)
    {
        this.StorageHolder = DirectoryStorageHolder.WorkingDirectoryHolder;
        this.Name = relativePath;
    }
    
    public StorageSlot(IStorageHolder storageHolder, string relativePath) : this(relativePath)
    {
        this.StorageHolder = storageHolder;
    }

    public virtual string? FullName => Path.Combine(StorageHolder.FullName, Name);

    public IStorageHolder? StorageHolder { get; protected set; }
    public virtual string Name { get; protected set; }

    protected IRawData RawData { get; set; }

    public virtual IRawData? GetData()
    {
        if (RawData != null)
        {
            return RawData;
        }

        string filePath = FullName;
        if (File.Exists(filePath))
        {
            RawData = new RawData(File.ReadAllBytes(filePath));
        }
        else
        {
            throw new ArgumentException($"slot not found {FullName}");
        }

        return RawData;
    }


    public abstract void SetData(IRawData rawData);
    
    public static IStorageSlot GetSegmentedPathStorageSlot(IStorageHolder rootHolder, string hashHexString)
    {
        Args.ThrowIfNullOrEmpty(hashHexString, nameof(hashHexString));
        
        List<string> parts = new List<string>();
        parts.AddRange(hashHexString.Split(2));
        parts.Add("dat");
        return new FsStorageSlot(rootHolder, Path.Combine(parts.ToArray()));
    }
}