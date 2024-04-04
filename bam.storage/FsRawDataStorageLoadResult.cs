namespace Bam.Storage;

public class FsRawDataStorageLoadResult : IRawDataStorageLoadResult, IResult
{
    public FsRawDataStorageLoadResult(string path, IRawData rawData)
    {
        this.Path = path;
        this.Success = true;
        this.RawData = rawData;
    }

    public FsRawDataStorageLoadResult(Exception ex)
    {
        this.Success = false;
        this.SetMessage(ex);
    }
    
    public IRawData RawData { get; }
    public bool Success { get; }
    public string Message { get; set; }
    public string Path { get; set; }
}