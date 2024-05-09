using Bam.Net;

namespace Bam.Storage;

public class DirectoryStorageContainer : FsStorageContainer, IStorageContainer
{
    public DirectoryStorageContainer() : base(BamProfile.DataPath)
    {
    }

    public DirectoryStorageContainer(string path) : base(path)
    {
    }

    public DirectoryStorageContainer(DirectoryInfo directory) : base(directory)
    {
    }

    private static DirectoryStorageContainer _workingDirectoryContainer;
    private static readonly object _workingDirectoryContainerLock = new object();
    public static DirectoryStorageContainer WorkingDirectoryContainer
    {
        get
        {
            return _workingDirectoryContainerLock.DoubleCheckLock(ref _workingDirectoryContainer,
                () => new DirectoryStorageContainer(BamDir.Data));
        }
    }

    private static DirectoryStorageContainer _profileDirectoryContainer;
    private static readonly object _profileDirectoryContainerLock = new object();
    public static DirectoryStorageContainer ProfileDirectoryContainer
    {
        get
        {
            return _profileDirectoryContainerLock.DoubleCheckLock(ref _profileDirectoryContainer,
                () => new DirectoryStorageContainer(BamProfile.DataPath));
        }
    }
    
    public IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(this, relativePath);
    }
}