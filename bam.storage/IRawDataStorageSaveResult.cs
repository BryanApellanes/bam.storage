namespace Bam.Storage;

/// <summary>
/// Represents the result of saving raw data to storage, including a reference to the saved data and status information.
/// </summary>
public interface IRawDataStorageSaveResult
{
    /// <summary>
    /// Gets the raw data that was saved.
    /// </summary>
    IRawData RawData { get; }

    /// <summary>
    /// Gets a value indicating whether the save operation completed successfully.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets a message describing the result, typically containing error details on failure.
    /// </summary>
    string Message { get; }
}