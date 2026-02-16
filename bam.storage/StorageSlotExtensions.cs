namespace Bam.Storage;

/// <summary>
/// Provides extension methods for <see cref="IStorageSlot"/> instances.
/// </summary>
public static class StorageSlotExtensions
{
    /// <summary>
    /// Creates a <see cref="NormalizedSlotInfo"/> containing both the original and normalized (fully resolved) holder and slot.
    /// </summary>
    /// <param name="slot">The storage slot to normalize.</param>
    /// <returns>A <see cref="NormalizedSlotInfo"/> with original and normalized references.</returns>
    public static NormalizedSlotInfo GetNormalizedSlotInfo(this IStorageSlot slot)
    {
        Args.ThrowIfNull(slot, "slot");
        Args.ThrowIfNull(slot.StorageHolder, $"slot.{nameof(slot.StorageHolder)}");
        SlotInfo slotInfo = new SlotInfo(slot);
        return new NormalizedSlotInfo()
        {
            Holder = slot.StorageHolder,
            Slot = slot,
            NormalizedHolder = slotInfo.StorageHolder,
            NormalizedSlot = slotInfo
        };
    }

    /// <summary>
    /// Gets the file name portion of the slot's full path.
    /// </summary>
    /// <param name="slot">The storage slot.</param>
    /// <returns>The file name portion of the slot's full path.</returns>
    public static string GetName(this IStorageSlot slot)
    {
        string fullPath = GetFullPath(slot);
        return Path.GetFileName(fullPath);
    }

    /// <summary>
    /// Gets the full path of the slot by combining the holder's path with the slot's name.
    /// Falls back to the profile directory holder if no holder is specified, and defaults to "dat" if no name is set.
    /// </summary>
    /// <param name="slot">The storage slot.</param>
    /// <returns>The fully resolved path of the storage slot.</returns>
    public static string GetFullPath(this IStorageSlot slot)
    {
        string slotPath = slot.FullName;
        if (string.IsNullOrEmpty(slotPath))
        {
            slotPath = slot.Name;
        }

        if (string.IsNullOrEmpty(slotPath))
        {
            slotPath = "dat";
        }
        
        IStorageHolder holder = slot.StorageHolder ?? DirectoryStorageHolder.ProfileDirectoryHolder;

        return Path.Combine(holder.FullName, slotPath);
    }
}