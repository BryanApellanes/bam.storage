namespace Bam.Storage;

/// <summary>
/// Represents the result of saving raw data to the file system, including the file path and success status.
/// </summary>
public class FsRawDataDataStorageSaveResult: IFsRawDataDataStorageSaveResult, IResult
{
    /// <summary>
    /// Initializes a successful save result with the specified file path and raw data.
    /// </summary>
    /// <param name="path">The file system path where the data was saved.</param>
    /// <param name="rawData">The raw data that was saved.</param>
    public FsRawDataDataStorageSaveResult(string path, IRawData rawData)
    {
        this.RawData = rawData;
        this.Path = path;
        this.Success = true;
    }

    /// <summary>
    /// Initializes a failed save result from the specified exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    public FsRawDataDataStorageSaveResult(Exception ex)
    {
        this.Success = false;
        this.SetMessage(ex);
    }

    /// <summary>
    /// Gets the raw data that was saved, or null if the operation failed.
    /// </summary>
    public IRawData RawData { get; }

    /// <summary>
    /// Gets a value indicating whether the save operation completed successfully.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets or sets a message describing the result, typically containing error details on failure.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets the file system path where the data was saved, or null if the operation failed.
    /// </summary>
    public string Path { get; }
}