namespace Bam.Storage;

public interface IKeyValuePair
{
    string Key { get; set; }
    byte[] Value { get; set; }
}