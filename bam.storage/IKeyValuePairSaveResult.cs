namespace Bam.Storage;

public interface IKeyValuePairSaveResult
{
    string Key { get; }
    bool Success { get; }
    string ErrorMessage { get; }
}