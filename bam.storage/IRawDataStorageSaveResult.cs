namespace Bam.Storage;

public interface IRawDataStorageSaveResult
{
    IRawData RawData { get; }
    bool Success { get; }
    string Message { get; }
}