namespace Bam.Storage.Encryption;

/// <summary>
/// A file-system storage slot that carries a reference to an <see cref="OpaquenessProvider"/>,
/// enabling encryption-aware storage operations at the slot level.
/// </summary>
public class OpaqueFsStorageSlot : FsStorageSlot
{
    /// <summary>
    /// Initializes a new instance of <see cref="OpaqueFsStorageSlot"/> with the specified opaqueness provider,
    /// storage holder, and relative path.
    /// </summary>
    /// <param name="opaquenessProvider">The provider for encryption and hash transformation operations.</param>
    /// <param name="storageHolder">The storage holder containing this slot.</param>
    /// <param name="relativePath">The relative path of this slot within the holder.</param>
    public OpaqueFsStorageSlot(OpaquenessProvider opaquenessProvider, IStorageHolder storageHolder, string relativePath)
        : base(storageHolder, relativePath)
    {
        this.OpaquenessProvider = opaquenessProvider;
    }

    /// <summary>
    /// Gets or sets the opaqueness provider used for encryption and hash transformation operations.
    /// </summary>
    protected internal OpaquenessProvider OpaquenessProvider { get; set; }

}