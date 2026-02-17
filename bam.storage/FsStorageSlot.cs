namespace Bam.Storage;

/// <summary>
/// A file-system-based storage slot that reads and writes raw data as files on disk.
/// Creates the parent directory structure automatically when writing data.
/// </summary>
public class FsStorageSlot : StorageSlot
{
    /// <summary>
    /// Initializes a new instance of <see cref="FsStorageSlot"/> with the default relative path "dat".
    /// </summary>
    public FsStorageSlot(): this("dat")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsStorageSlot"/> with the specified relative path
    /// and the working directory as the storage holder.
    /// </summary>
    /// <param name="relativePath">The relative path (name) of this slot.</param>
    public FsStorageSlot(string relativePath) : base(relativePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FsStorageSlot"/> with the specified storage holder and relative path.
    /// </summary>
    /// <param name="storageHolder">The storage holder that contains this slot.</param>
    /// <param name="relativePath">The relative path (name) of this slot within the holder.</param>
    public FsStorageSlot(IStorageHolder storageHolder, string relativePath) : base(storageHolder, relativePath)
    {
    }

    /// <summary>
    /// Writes the specified raw data to this slot's file path, creating the directory structure if it does not exist.
    /// </summary>
    /// <param name="rawData">The raw data to write to disk.</param>
    public override void SetData(IRawData rawData)
    {
        string filePath = FullName!;
        FileInfo fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            fileInfo.Directory!.Create();
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