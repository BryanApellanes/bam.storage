using Bam.Net;

namespace Bam.Storage;

public static class StorageSlotExtensions
{
    public static NormalizedSlotInfo GetNormalizedSlotInfo(this IStorageSlot slot)
    {
        Args.ThrowIfNull(slot, "slot");
        Args.ThrowIfNull(slot.StorageContainer, $"slot.{nameof(slot.StorageContainer)}");
        SlotInfo slotInfo = new SlotInfo(slot);
        return new NormalizedSlotInfo()
        {
            Container = slot.StorageContainer,
            Slot = slot,
            NormalizedContainer = slotInfo.StorageContainer,
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
        
        IStorageContainer container = slot.StorageContainer ?? DirectoryStorageContainer.ProfileDirectoryContainer;

        return Path.Combine(container.FullName, slotPath);
    }
}