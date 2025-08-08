namespace Bam.Storage;

public interface IKeyValue
{
    string Key { get; set; }
    byte[] Value { get; set; }
}