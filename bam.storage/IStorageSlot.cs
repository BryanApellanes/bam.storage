namespace Bam.Storage;

public interface IStorageSlot : IStorageIdentifier
{
    IStorageHolder? StorageHolder { get; }
    string RelativePath { get; }
    string Name { get; }
    IRawData? GetData();
    void SetData(IRawData rawData);
}