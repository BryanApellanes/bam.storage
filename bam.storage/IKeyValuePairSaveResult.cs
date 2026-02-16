namespace Bam.Storage;

/// <summary>
/// Represents the result of saving a key-value pair to storage.
/// </summary>
public interface IKeyValuePairSaveResult
{
    /// <summary>
    /// Gets the key that was saved.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets a value indicating whether the save operation completed successfully.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets the error message if the save operation failed.
    /// </summary>
    string ErrorMessage { get; }
}