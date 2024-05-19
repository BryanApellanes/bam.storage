using Bam.Net;

namespace Bam.Storage;

public static class StorageSlotExtensions
{
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
    
    public static string GetName(this IStorageSlot slot)
    {
        string fullPath = GetFullPath(slot);
        return Path.GetFileName(fullPath);
    }

    public static string GetFullPath(this IStorageSlot slot)
    {
        string slotPath = slot.RelativePath;
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