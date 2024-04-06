namespace Bam.Storage;

public class DirectoryStorageContainer : FsStorageIdentifier, IStorageContainer
{
    public DirectoryStorageContainer(string path) : base(path)
    {
    }

    public DirectoryStorageContainer(DirectoryInfo directory) : base(directory)
    {
    }
    
    public string? FullName { get; }
}