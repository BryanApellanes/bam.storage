namespace Bam.Storage;

public class RootStorageContainer : DirectoryStorageContainer, IRootStorageContainer
{
    public RootStorageContainer(string path) : base(path)
    {
    }

    public RootStorageContainer(DirectoryInfo directory) : base(directory)
    {
    }
}