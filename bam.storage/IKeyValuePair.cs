namespace Bam.Storage;

/// <summary>
/// Represents a key-value pair where the key is a string and the value is a byte array.
/// </summary>
public interface IKeyValuePair
{
    /// <summary>
    /// Gets or sets the string key that identifies this pair.
    /// </summary>
    string Key { get; set; }

    /// <summary>
    /// Gets or sets the byte array value associated with the key.
    /// </summary>
    byte[] Value { get; set; }
}