namespace Bam.Storage;

/// <summary>
/// Represents a reference to raw data identified by its hash string, without necessarily holding the actual data in memory.
/// </summary>
public class RawDataReference : RawData
{
    /// <summary>
    /// Initializes a new instance of <see cref="RawDataReference"/> using the specified hash string as the data content.
    /// </summary>
    /// <param name="hashString">The hash string identifying the referenced data.</param>
    public RawDataReference(string hashString) : base(hashString)
    {
    }
}