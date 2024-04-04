using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam.Net;

namespace Bam.Storage;

public class RawData : IRawData
{
    protected RawData(): this(null, Encoding.UTF8)
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

    public ulong HashId => BitConverter.ToUInt64(Hash, 0);
    public string HashString => Hash.ToHexString();
    public byte[] Hash => Value.HashBytes(this.HashAlgorithm);
    public byte[] Value { get; protected init; }

    public override string ToString()
    {
        return Encoding.GetString(this.Value);
    }
}