namespace Bam.Storage;

/// <summary>
/// Contains both the original and normalized versions of a storage holder and slot pair,
/// providing fully resolved path information alongside the original references.
/// </summary>
public class NormalizedSlotInfo
{
    /// <summary>
    /// Gets or sets the original storage holder before normalization.
    /// </summary>
    public IStorageHolder Holder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the original storage slot before normalization.
    /// </summary>
    public IStorageSlot Slot { get; set; } = null!;

    /// <summary>
    /// Gets or sets the normalized storage holder with fully resolved path information.
    /// </summary>
    public IStorageHolder NormalizedHolder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the normalized storage slot with fully resolved path information.
    /// </summary>
    public IStorageSlot NormalizedSlot { get; set; } = null!;
}