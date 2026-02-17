
namespace Bam.Storage;

/// <summary>
/// Represents the result of saving a key-value pair to storage, indicating success or failure.
/// </summary>
public class KeyValuePairSaveResult: IKeyValuePairSaveResult
{
    /// <summary>
    /// Gets or sets the key that was saved.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the save operation completed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the save operation failed.
    /// </summary>
    public string ErrorMessage { get; set; } = null!;
}