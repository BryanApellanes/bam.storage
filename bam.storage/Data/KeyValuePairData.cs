using Bam.Data.Repositories;

namespace Bam.Storage.Data;

/// <summary>
/// A data transfer object implementing <see cref="IKeyValuePair"/> for use in data layer operations.
/// </summary>
public class KeyValuePairData : IKeyValuePair //, AuditRepoData
{
    /// <summary>
    /// Gets or sets the string key that identifies this pair.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Gets or sets the byte array value associated with the key.
    /// </summary>
    public byte[] Value { get; set; } = null!;
}