namespace Bam.Storage;

public class RawDataKeyValuePair : IKeyValuePair
{
    public RawDataKeyValuePair(IRawData rawData)
    {
        this.Key = rawData.HashHexString;
        this.Value = rawData.Value;
    }
    public string Key { get; set; }
    public byte[] Value { get; set; }
}