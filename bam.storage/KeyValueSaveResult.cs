
namespace Bam.Storage;

public class KeyValueSaveResult: IKeyValueSaveResult
{
    public string Key { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}