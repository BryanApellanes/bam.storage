namespace Bam.Storage;

/// <summary>
/// Represents the outcome of a storage operation, indicating success or failure with an optional message.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets or sets a message describing the result, typically containing error details on failure.
    /// </summary>
    string Message { get; set; }
}