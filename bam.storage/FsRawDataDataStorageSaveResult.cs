using Bam;

namespace Bam.Storage;

public class FsRawDataDataStorageSaveResult: IFsRawDataDataStorageSaveResult, IResult
{
    public FsRawDataDataStorageSaveResult(string path, IRawData rawData)
    {
        this.RawData = rawData;
        this.Path = path;
        this.Success = true;
    }

    public FsRawDataDataStorageSaveResult(Exception ex)
    {
        this.Success = false;
        this.SetMessage(ex);
    }
    
    public IRawData RawData { get; }
    public bool Success { get; }
    public string Message { get; set; }
    public string Path { get; }
}