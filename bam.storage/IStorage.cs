namespace Bam.Storage;

public interface IStorage : IRawStorage
{
    IStorageHolder RootHolder { get; }
    IStorageSlot CurrentSlot { get; set; }
    IStorageSlot GetSlot(string relativePath);
    IStorageSlot Save(IStorageSlot slot, IRawData rawData);
    IStorageSlot Save(string relativePath, IRawData rawData);
   
    IStorageSlot Save(byte[] data);
    IStorageSlot Save(IStorageSlot slot, byte[] data);
    IStorageSlot Save(string relativePath, byte[] data);

    IRawData Load(IStorageSlot slot);
    IRawData Load(string relativePath);
}