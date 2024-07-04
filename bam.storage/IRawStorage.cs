namespace Bam.Storage;

public interface IRawStorage
{
    IStorageSlot Save(IRawData rawData);
    IRawData LoadHashId(ulong hashId);
    IRawData LoadHashString(string hashString);
}