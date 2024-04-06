using Bam.Storage;

namespace bam.storage;

public class FileStorageSlot : RawData, IStorageSlot
{
    public FileInfo File { get; set; }
    public string? FullName { get; }
}