namespace Bam.Storage;

public abstract class Storage : IStorage
{
    public abstract IStorageHolder RootHolder { get; }
    public abstract IStorageSlot CurrentSlot { get; set; }
    public abstract IStorageSlot GetSlot();
    public abstract IStorageSlot GetSlot(string relativePath);
    public abstract IStorageSlot Save(IStorageSlot slot, IRawData rawData);
    public abstract IStorageSlot Save(IRawData rawData);
    public abstract IStorageSlot Save(string relativePath, IRawData rawData);

    public abstract IStorageSlot Save(byte[] data);
    public abstract IStorageSlot Save(IStorageSlot slot, byte[] data);

    public abstract IStorageSlot Save(string relativePath, byte[] data);
    public abstract IRawData LoadSlot(IStorageSlot slot);
    public abstract IRawData LoadHashId(ulong hashId);
    public abstract IRawData LoadHashString(string hashString);
    public abstract IRawData Load(string relativePath);


    protected virtual void WriteBytes(string path, byte[] data)
    {
        File.WriteAllBytes(path, data);
    }

    protected virtual byte[] ReadBytes(string path)
    {
        return File.ReadAllBytes(path);
    }
}