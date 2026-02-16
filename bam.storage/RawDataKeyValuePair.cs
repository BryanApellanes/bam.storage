namespace Bam.Storage;

/// <summary>
/// A key-value pair derived from raw data, using the data's hex-encoded hash as the key and the raw bytes as the value.
/// </summary>
public class RawDataKeyValuePair : IKeyValuePair
{
    /// <summary>
    /// Initializes a new instance of <see cref="RawDataKeyValuePair"/> using the hash hex string of the raw data as the key.
    /// </summary>
    /// <param name="rawData">The raw data to create the key-value pair from.</param>
    public RawDataKeyValuePair(IRawData rawData)
    {
        this.Key = rawData.HashHexString;
        this.Value = rawData.Value;
    }

    /// <summary>
    /// Gets or sets the hex-encoded hash string of the raw data.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the raw byte data.
    /// </summary>
    public byte[] Value { get; set; }
}