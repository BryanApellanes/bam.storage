namespace Bam.Storage;

public class FsSlottedStorage : SlottedStorage
{
    public static implicit operator DirectoryInfo(FsSlottedStorage slottedStorage)
    {
        return slottedStorage.Directory;
    }
    
    public FsSlottedStorage()
    {
        this.Directory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "storage"));
        this.RootHolder = new FsStorageHolder(this.Directory);
    }

    public FsSlottedStorage(string path)
    {
        this.Directory = new DirectoryInfo(path);
        this.RootHolder = new FsStorageHolder(this.Directory);
    }
    
    public DirectoryInfo Directory { get; }

    public override IStorageHolder RootHolder { get; }
    public override IStorageSlot CurrentSlot { get; set; }

    public override IStorageSlot GetSlot()
    {
        return CurrentSlot ?? GetSlot("dat");
    }

    public override IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(RootHolder, relativePath);
    }

    public override IStorageSlot Save(IRawData data)
    {
        return Save(this.CurrentSlot ?? this.GetHashHexStringStorageSlot(data.HashHexString), data);
    }
    
    public override IStorageSlot Save(byte[] data)
    {
        RawData rawData = new RawData(data);
        return Save(rawData);
    }

    public override IStorageSlot Save(IStorageSlot slot, byte[] data)
    {
        return this.Save(slot, new RawData(data));
    }

    public override IStorageSlot Save(string relativePath, byte[] data)
    {
        return Save(relativePath, new RawData(data));
    }

    public override IStorageSlot Save(string relativePath, IRawData rawData)
    {
        return Save(this.GetSlot(relativePath), rawData);
    } 
    
    public override IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        Args.ThrowIfNull(RootHolder, nameof(RootHolder));
        Args.ThrowIf(!slot.StorageHolder.FullName.StartsWith(RootHolder.FullName, StringComparison.InvariantCultureIgnoreCase), $"{nameof(FsSlottedStorage)}:: slot is in {slot.StorageHolder.FullName} not in storage root {RootHolder.FullName}");
        
        slot.SetData(rawData);
        return slot;
    }
    
    public override IRawData LoadHashHexString(string hashHexString)
    {
        return LoadSlot(GetHashHexStringStorageSlot(hashHexString));
    }

    public override IRawData Load(string relativePath)
    {
        return LoadSlot(GetSlot(relativePath));
    }

    public override IRawData LoadSlot(IStorageSlot slot)
    {
        return slot.GetData();
    }
    
    public virtual IStorageSlot GetHashHexStringStorageSlot(string hashHexString)
    {
        return FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hashHexString);
    }
}