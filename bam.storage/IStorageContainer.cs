using Bam.Storage;

namespace Bam.Storage;

public interface IStorageContainer : IStorageIdentifier
{
    IStorageSlot GetSlot(string relativePath);
}