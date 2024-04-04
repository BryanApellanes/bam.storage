namespace Bam.Storage;

public interface IRawDataStorageLoadResult
{
    IRawData RawData { get; }
    bool Success { get; }
    string Message { get; }
}