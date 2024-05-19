namespace Bam.Storage;

public class HolderInfo: IStorageHolder
{
    public HolderInfo(): this(Path.Combine(DirectoryStorageHolder.ProfileDirectoryHolder, "dat"))
    {
    }

    public HolderInfo(string fullSlotPath)
    {
        this.FullSlotPath = fullSlotPath;
        this.FullName = Path.GetDirectoryName(fullSlotPath);
    }
    public string FullSlotPath { get; }
    public string? FullName { get; }
    
    public IStorageSlot GetSlot(string relativePath)
    {
        string fullPath = Path.Combine(FullName, relativePath);
        return new FsStorageSlot(this, relativePath);
    }
}