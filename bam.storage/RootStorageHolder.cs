namespace Bam.Storage;

public class RootStorageHolder : DirectoryStorageHolder, IRootStorageHolder
{
    public RootStorageHolder(string path) : base(path)
    {
    }

    public RootStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }
}