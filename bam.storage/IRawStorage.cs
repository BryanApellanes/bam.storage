namespace Bam.Storage;

public interface IRawStorage
{
    IStorageSlot Save(IRawData rawData);
    IRawData LoadHashId(ulong hashId);
    IRawData LoadHashHexString(string hashHexString);
}