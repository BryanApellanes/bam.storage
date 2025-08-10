
namespace Bam.Storage;

public class KeyValuePairSaveResult: IKeyValuePairSaveResult
{
    public string Key { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}