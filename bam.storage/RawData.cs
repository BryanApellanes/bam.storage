using System.Text;

namespace Bam.Storage;

public class RawData : IRawData
{
    protected RawData(): this(new byte[]{}, Encoding.UTF8)
    {
    }

    public RawData(string data, Encoding encoding): this(encoding.GetBytes(data))
    {
    }
    
    public RawData(string data) : this(Encoding.UTF8.GetBytes(data), Encoding.UTF8)
    {
    }

    public RawData(byte[] value): this(value, Encoding.UTF8)
    {
    }

    public RawData(byte[] value, Encoding encoding)
    {
        this.HashAlgorithm = HashAlgorithms.SHA256;
        this.Encoding = encoding ?? Encoding.UTF8;
        this.Value = value;
    }
    
    public HashAlgorithms HashAlgorithm { get; private set; }
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

    public override string ToString()
    {
        return Encoding.GetString(this.Value);
    }
}