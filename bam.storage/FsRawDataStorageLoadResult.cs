namespace Bam.Storage;

/// <summary>
/// Represents the result of loading raw data from the file system, including the file path, loaded data, and success status.
/// </summary>
public class FsRawDataStorageLoadResult : IRawDataStorageLoadResult, IResult
{
    /// <summary>
    /// Initializes a successful load result with the specified file path and raw data.
    /// </summary>
    /// <param name="path">The file system path the data was loaded from.</param>
    /// <param name="rawData">The raw data that was loaded.</param>
    public FsRawDataStorageLoadResult(string path, IRawData rawData)
    {
        this.Path = path;
        this.Success = true;
        this.RawData = rawData;
    }

    /// <summary>
    /// Initializes a failed load result from the specified exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    public FsRawDataStorageLoadResult(Exception ex)
    {
        this.Success = false;
        this.SetMessage(ex);
    }

    /// <summary>
    /// Gets the raw data that was loaded, or null if the operation failed.
    /// </summary>
    public IRawData RawData { get; } = null!;

    /// <summary>
    /// Gets a value indicating whether the load operation completed successfully.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets or sets a message describing the result, typically containing error details on failure.
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Gets or sets the file system path the data was loaded from.
    /// </summary>
    public string Path { get; set; } = null!;
}