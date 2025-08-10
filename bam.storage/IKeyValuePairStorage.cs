namespace Bam.Storage;

public interface IKeyValuePairStorage
{
    IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair);
    IKeyValuePair Get(string key);
}