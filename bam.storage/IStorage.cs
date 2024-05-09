namespace Bam.Storage;

public interface IStorage
{
    IStorageContainer RootContainer { get; }
    IStorageSlot Save(IRawData rawData);
    IStorageSlot Save(string relativePath, IRawData rawData);
    IStorageSlot Save(byte[] data);
    IStorageSlot Save(string relativePath, byte[] data);
    IRawData Load(string hashIdString);
    IRawData Load(ulong hashId);
}