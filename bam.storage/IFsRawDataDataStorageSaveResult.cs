namespace Bam.Storage;

public interface IFsRawDataDataStorageSaveResult : IRawDataStorageSaveResult
{
    string Path { get; }
}