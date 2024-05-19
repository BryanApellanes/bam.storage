using Bam.Net;

namespace Bam.Storage;

public class DirectoryStorageHolder : FsStorageHolder, IStorageHolder
{
    public DirectoryStorageHolder() : base(BamProfile.DataPath)
    {
    }

    public DirectoryStorageHolder(string path) : base(path)
    {
    }

    public DirectoryStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }

    private static DirectoryStorageHolder _workingDirectoryHolder;
    private static readonly object _workingDirectoryHolderLock = new object();
    public static DirectoryStorageHolder WorkingDirectoryHolder
    {
        get
        {
            return _workingDirectoryHolderLock.DoubleCheckLock(ref _workingDirectoryHolder,
                () => new DirectoryStorageHolder(BamDir.Data));
        }
    }

    private static DirectoryStorageHolder _profileDirectoryHolder;
    private static readonly object _profileDirectoryContainerLock = new object();
    public static DirectoryStorageHolder ProfileDirectoryHolder
    {
        get
        {
            return _profileDirectoryContainerLock.DoubleCheckLock(ref _profileDirectoryHolder,
                () => new DirectoryStorageHolder(BamProfile.DataPath));
        }
    }
    
    public IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(this, relativePath);
    }
}