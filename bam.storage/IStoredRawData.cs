
namespace Bam.Storage;

public interface IStoredRawData : IRawData
{
    IStorageSlot StorageSlot { get; set; }
}