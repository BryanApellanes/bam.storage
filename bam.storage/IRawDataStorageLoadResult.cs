namespace Bam.Storage;

/// <summary>
/// Represents the result of loading raw data from storage, including the loaded data and status information.
/// </summary>
public interface IRawDataStorageLoadResult
{
    /// <summary>
    /// Gets the raw data that was loaded from storage.
    /// </summary>
    IRawData RawData { get; }

    /// <summary>
    /// Gets a value indicating whether the load operation completed successfully.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets a message describing the result, typically containing error details on failure.
    /// </summary>
    string Message { get; }
}