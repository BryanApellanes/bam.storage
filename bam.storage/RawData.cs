using System.Text;

namespace Bam.Storage;

/// <summary>
/// Represents raw binary data with an associated SHA256 hash, supporting content-addressable storage patterns.
/// </summary>
public class RawData : IRawData
{
    /// <summary>
    /// Initializes a new instance of <see cref="RawData"/> with empty byte data and UTF-8 encoding.
    /// </summary>
    protected RawData(): this(new byte[]{}, Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RawData"/> from a string using the specified encoding.
    /// </summary>
    /// <param name="data">The string data to store.</param>
    /// <param name="encoding">The encoding used to convert the string to bytes.</param>
    public RawData(string data, Encoding encoding): this(encoding.GetBytes(data))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RawData"/> from a string using UTF-8 encoding.
    /// </summary>
    /// <param name="data">The string data to store.</param>
    public RawData(string data) : this(Encoding.UTF8.GetBytes(data), Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RawData"/> from a byte array using UTF-8 encoding.
    /// </summary>
    /// <param name="value">The raw byte data to store.</param>
    public RawData(byte[] value): this(value, Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RawData"/> from a byte array and encoding.
    /// </summary>
    /// <param name="value">The raw byte data to store.</param>
    /// <param name="encoding">The encoding used for string conversions.</param>
    public RawData(byte[] value, Encoding encoding)
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
        this.Encoding = encoding ?? Encoding.UTF8;
        this.Value = value;
    }

    /// <summary>
    /// Gets the hash algorithm used to compute the hash of the data. Defaults to SHA256.
    /// </summary>
    public HashAlgorithms HashAlgorithm { get; private set; }

    /// <summary>
    /// Gets the text encoding used for string conversions of this data.
    /// </summary>
    public Encoding Encoding  { get; protected init; }

    /// <summary>
    /// Gets the hash converted to an unsigned long.
    /// </summary>
    public ulong HashId => BitConverter.ToUInt64(Hash, 0);

    private string _hashString;
    /// <summary>
    /// Gets the hash hex string equivalent.
    /// </summary>
    public string HashHexString
    {
        get
        {
            if (string.IsNullOrEmpty(_hashString) && Hash is { Length: > 0 })
            {
                _hashString = Hash.ToHexString();
            }

            return _hashString;
        }
        set => _hashString = value;
    }

    private byte[] _hash;
    /// <summary>
    /// Gets the binary hash.
    /// </summary>
    public byte[] Hash 
    {
        get
        {
            if (Value != null)
            {
                _hash = Value.HashBytes(this.HashAlgorithm);
            }
            
            if (_hash == null && !string.IsNullOrEmpty(HashHexString))
            {
                _hash = HashHexString.HexToBytes();
            }

            return _hash;
        }
        set => _hash = value;
    }
    
    /// <summary>
    /// Gets the raw value.
    /// </summary>
    public byte[] Value { get; protected init; }

    /// <summary>
    /// Returns the string representation of the raw data using the configured encoding.
    /// </summary>
    /// <returns>The string representation of the stored byte data.</returns>
    public override string ToString()
    {
        return Encoding.GetString(this.Value);
    }
}