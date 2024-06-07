namespace Bam.Storage;

public interface IRawStorage
{
    IStorageSlot Save(IRawData rawData);
    IRawData Load(ulong hashId);
}