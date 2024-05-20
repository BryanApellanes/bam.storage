namespace Bam.Storage;

public abstract class Storage : IStorage
{
    public abstract IStorageHolder RootHolder { get; }
    public abstract IStorageSlot Save(IRawData rawData);
    public abstract IStorageSlot Save(string relativePath, IRawData rawData);

    public abstract IStorageSlot Save(byte[] data);

    public abstract IStorageSlot Save(string relativePath, byte[] data);

    public abstract IRawData Load(string hashIdString);


    protected virtual void WriteBytes(string path, byte[] data)
    {
        File.WriteAllBytes(path, data);
    }

    protected virtual byte[] ReadBytes(string path)
    {
        return File.ReadAllBytes(path);
    }
}