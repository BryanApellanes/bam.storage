namespace Bam.Storage;

public interface IKeyValueStorage
{
    IKeyValueSaveResult Save(IKeyValue keyValue);
    IKeyValue Get(string key);
}