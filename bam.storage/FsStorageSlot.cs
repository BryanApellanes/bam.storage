using Bam.Net;

namespace Bam.Storage;

public class FsStorageSlot : RawData, IStorageSlot
{
    public FsStorageSlot(FileInfo file)
    {
        Args.ThrowIfNull(file, "file");
        this.File = file;
    }

    private FileInfo File { get; }

    public string? FullName => this.File?.FullName;

    private IStorageContainer _storageContainer;
    public IStorageContainer StorageContainer => _storageContainer ?? (_storageContainer = new DirectoryStorageContainer(this.File?.Directory));

    public string Name => this.File.Name;
    public IStorageSlot Save(IStorage storage, IRawData rawData)
    {
        throw new NotImplementedException();
    }
}