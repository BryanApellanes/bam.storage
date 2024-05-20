namespace Bam.Storage;

public interface IStorageSlot : IStorageIdentifier
{
    IStorageHolder? StorageHolder { get; }
    string Name { get; }
    IRawData? GetData();
    void SetData(IRawData rawData);
}