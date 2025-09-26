namespace Bam.Storage;

public class FsStorageSlot : StorageSlot
{
    public FsStorageSlot(): this("dat")
    {
    }

    public FsStorageSlot(string relativePath) : base(relativePath)
    {
    }
    
    public FsStorageSlot(IStorageHolder storageHolder, string relativePath) : base(storageHolder, relativePath)
    {
    }

    public override void SetData(IRawData rawData)
    {
        string filePath = FullName;
        FileInfo fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            fileInfo.Directory.Create();
        }
        File.WriteAllBytes(filePath, rawData.Value);
        this.RawData = rawData;
    }
    
    /*public static IStorageSlot GetSegmentedPathStorageSlot(IStorageHolder rootHolder, string hashHexString)
    {
        Args.ThrowIfNullOrEmpty(hashHexString, nameof(hashHexString));
        
        List<string> parts = new List<string>();
        parts.AddRange(hashHexString.Split(2));
        parts.Add("dat");
        return new FsStorageSlot(rootHolder, Path.Combine(parts.ToArray()));
    }*/
}