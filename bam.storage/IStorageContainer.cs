using Bam.Storage;

namespace Bam.Storage;

public interface IStorageContainer : IStorageIdentifier
{
    IStorageSlot Save(IStorage storage, IRawData rawData);
    IStorageSlot Save(IStorage storage, string relativePath, IRawData rawData);
    IStorageSlot GetSlot(string relativePath);
}