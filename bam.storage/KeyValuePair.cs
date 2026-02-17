
using System.Text;

namespace Bam.Storage;

/// <summary>
/// A key-value pair where the key is a string and the value is a byte array.
/// String values are automatically converted to bytes using UTF-8 encoding.
/// </summary>
public class KeyValuePair : IKeyValuePair
{
    /// <summary>
    /// Initializes a new empty instance of <see cref="KeyValuePair"/>.
    /// </summary>
    public KeyValuePair()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="KeyValuePair"/> with the specified key and string value,
    /// encoding the value as UTF-8 bytes.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The string value to encode as UTF-8 bytes.</param>
    public KeyValuePair(string key, string value) : this(key, Encoding.UTF8.GetBytes(value))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="KeyValuePair"/> with the specified key and byte array value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The byte array value.</param>
    public KeyValuePair(string key, byte[] value)
    {
        this.Key = key;
        this.Value = value;
    }

    /// <summary>
    /// Gets or sets the string key that identifies this pair.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Gets or sets the byte array value associated with the key.
    /// </summary>
    public byte[] Value { get; set; } = null!;
}