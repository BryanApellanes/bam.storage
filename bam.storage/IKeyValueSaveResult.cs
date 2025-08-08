namespace Bam.Storage;

public interface IKeyValueSaveResult
{
    string Key { get; }
    bool Success { get; }
    string ErrorMessage { get; }
}