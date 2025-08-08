
namespace Bam.Storage;

public class KeyValue : IKeyValue
{
    public string Key { get; set; }
    public byte[] Value { get; set; }
}