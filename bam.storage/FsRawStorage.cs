namespace Bam.Storage;

public class FsRawStorage : IRawStorage
{
    public FsRawStorage()
    {
        this.RootHolder = DirectoryStorageHolder.WorkingDirectoryHolder;
    }
    
    public FsRawStorage(IStorageHolder rootHolder)
    {
        this.RootHolder = rootHolder;
    }
    
    public FsRawStorage(string path)
    {
        this.RootHolder = new FsStorageHolder(path);
    }
    
    public virtual IStorageSlot GetSlot()
    {
        return CurrentSlot ?? GetSlot("dat");
    }

    public virtual IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(RootHolder, relativePath);
    }

    public IStorageHolder RootHolder { get; }
    public IStorageSlot CurrentSlot { get; set; }
    
    public virtual IStorageSlot Save(IRawData rawData)
    {
        return Save(this.CurrentSlot ?? FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, rawData.HashHexString), rawData);
    }
    
    public virtual IRawData LoadHashHexString(string hashHexString)
    {
        return FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hashHexString).GetData();
    }

    protected virtual IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        Args.ThrowIfNull(slot, nameof(slot));
        Args.ThrowIfNull(RootHolder, nameof(RootHolder));
        Args.ThrowIf(!slot.StorageHolder.FullName.StartsWith(RootHolder.FullName, StringComparison.InvariantCultureIgnoreCase), $"{nameof(FsRawStorage)}:: slot is in {slot.StorageHolder.FullName} not in storage root {RootHolder.FullName}");
        
        slot.SetData(rawData);
        return slot;
    }
}