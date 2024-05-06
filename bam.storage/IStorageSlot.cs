namespace Bam.Storage;

public interface IStorageSlot : IStorageIdentifier
{
    IStorageContainer StorageContainer { get; }
    string Name { get; }
    IStorageSlot Save(IStorage storage, IRawData rawData);
}