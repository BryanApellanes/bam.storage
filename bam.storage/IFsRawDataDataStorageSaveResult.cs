namespace Bam.Storage;

/// <summary>
/// Extends <see cref="IRawDataStorageSaveResult"/> with the file system path where the data was saved.
/// </summary>
public interface IFsRawDataDataStorageSaveResult : IRawDataStorageSaveResult
{
    /// <summary>
    /// Gets the file system path where the raw data was saved.
    /// </summary>
    string Path { get; }
}