namespace Bam.Storage;

public class RawDataKeyValue : IKeyValue
{
    public RawDataKeyValue(IRawData rawData)
    {
        this.Key = rawData.HashHexString;
        this.Value = rawData.Value;
    }
    public string Key { get; set; }
    public byte[] Value { get; set; }
}