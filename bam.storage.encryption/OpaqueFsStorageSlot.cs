namespace Bam.Storage.Encryption;

public class OpaqueFsStorageSlot : FsStorageSlot
{
    public OpaqueFsStorageSlot(OpaquenessProvider opaquenessProvider, IStorageHolder storageHolder, string relativePath)
        : base(storageHolder, relativePath)
    {
        this.OpaquenessProvider = opaquenessProvider;
    }
    
    protected internal OpaquenessProvider OpaquenessProvider { get; set; }

}