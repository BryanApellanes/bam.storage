namespace Bam.Storage;

/// <summary>
/// Represents a container that holds storage slots, such as a directory on the file system.
/// </summary>
public interface IStorageHolder : IStorageIdentifier
{
    /// <summary>
    /// Gets a storage slot at the specified relative path within this holder.
    /// </summary>
    /// <param name="relativePath">The relative path identifying the slot within this holder.</param>
    /// <returns>An <see cref="IStorageSlot"/> representing the storage location.</returns>
    IStorageSlot GetSlot(string relativePath);
}