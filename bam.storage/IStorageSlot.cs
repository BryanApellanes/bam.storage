namespace Bam.Storage;

public interface IStorageSlot : IStorageIdentifier
{
    IStorageContainer? StorageContainer { get; }
    string RelativePath { get; }
    string Name { get; }
    IRawData? GetData();
    void SetData(IRawData rawData);
}