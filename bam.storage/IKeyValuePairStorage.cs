namespace Bam.Storage;

public interface IKeyValuePairStorage
{
    IKeyValuePairSaveResult Save(string key, string value);
    IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair);
    IKeyValuePair Get(string key);
}