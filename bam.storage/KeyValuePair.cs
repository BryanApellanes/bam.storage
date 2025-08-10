
using System.Text;

namespace Bam.Storage;

public class KeyValuePair : IKeyValuePair
{
    public KeyValuePair()
    {
    }

    public KeyValuePair(string key, string value) : this(key, Encoding.UTF8.GetBytes(value))
    {
    }

    public KeyValuePair(string key, byte[] value)
    {
        this.Key = key;
        this.Value = value;
    }
    public string Key { get; set; }
    public byte[] Value { get; set; }
}