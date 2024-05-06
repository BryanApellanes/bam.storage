namespace Bam.Storage;

public class DirectoryStorageContainer : FsStorageIdentifier, IStorageContainer
{
    public DirectoryStorageContainer(string path) : base(path)
    {
    }

    public DirectoryStorageContainer(DirectoryInfo directory) : base(directory)
    {
    }


    public IStorageSlot Save(IStorage storage, IRawData rawData)
    {
        throw new NotImplementedException();
    }

    public IStorageSlot Save(IStorage storage, string relativePath, IRawData rawData)
    {
        throw new NotImplementedException();
    }

    public IStorageSlot GetSlot(string relativePath)
    {
        throw new NotImplementedException();
    }
}