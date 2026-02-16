namespace Bam.Storage;

/// <summary>
/// Identifies a storage location by its fully qualified name.
/// </summary>
public interface IStorageIdentifier
{
    /// <summary>
    /// Gets the fully qualified name of this storage location (for example, a full file system path).
    /// </summary>
    string FullName { get; }
}