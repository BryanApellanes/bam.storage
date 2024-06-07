using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam;

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
    
    /// <summary>
    /// Gets the hash hex string equivalent.
    /// </summary>
    public string HashString => Hash.ToHexString();
    
    /// <summary>
    /// Gets the 
    /// </summary>
    public byte[] Hash => Value.HashBytes(this.HashAlgorithm);
    
    /// <summary>
    /// Gets the raw value.
    /// </summary>
    public byte[] Value { get; protected init; }

    public T Convert<T>()
    {
        // string
        // boolean
        // uint64 aka ulong
        // char

        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Encoding.GetString(this.Value);
    }
}