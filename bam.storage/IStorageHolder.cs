using Bam.Storage;

namespace Bam.Storage;

public interface IStorageHolder : IStorageIdentifier
{
    IStorageSlot GetSlot(string relativePath);
}