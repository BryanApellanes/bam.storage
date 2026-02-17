namespace Bam.Storage;

/// <summary>
/// Wraps an existing <see cref="IStorageSlot"/> to provide normalized path information,
/// combining the holder and slot paths into a fully resolved location. Delegates data
/// operations to the original slot.
/// </summary>
public class SlotInfo : IStorageSlot
{
    /// <summary>
    /// Initializes a new instance of <see cref="SlotInfo"/> by normalizing the path of the specified slot.
    /// </summary>
    /// <param name="slot">The original storage slot to wrap and normalize.</param>
    public SlotInfo(IStorageSlot slot)
    {
        this.Original = slot;
        this.FullName = Path.Combine(slot.StorageHolder!.FullName!, slot.FullName!);
        this.Name = Path.GetFileName(this.FullName);
        this.RelativePath = this.Name;
        this.StorageHolder = new HolderInfo(slot.StorageHolder.FullName!);
    }

    private IStorageSlot Original { get; }

    /// <summary>
    /// Gets the fully resolved path combining the holder path and slot path.
    /// </summary>
    public string? FullName { get; }

    /// <summary>
    /// Gets the normalized storage holder derived from the original slot's holder path.
    /// </summary>
    public IStorageHolder? StorageHolder { get; }

    /// <summary>
    /// Gets the relative path of this slot, which is the file name portion of the full path.
    /// </summary>
    public string RelativePath { get; }

    /// <summary>
    /// Gets the file name portion of the normalized full path.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Reads and returns the raw data from the original underlying slot.
    /// </summary>
    /// <returns>The raw data from the original slot.</returns>
    public IRawData? GetData()
    {
        return this.Original.GetData();
    }

    /// <summary>
    /// Writes the specified raw data to the original underlying slot.
    /// </summary>
    /// <param name="rawData">The raw data to store.</param>
    public void SetData(IRawData rawData)
    {
        this.Original.SetData(rawData);
    }
}