namespace Bam.Storage;

public class NormalizedSlotInfo
{
    public IStorageHolder Holder { get; set; }
    public IStorageSlot Slot { get; set; }
    public IStorageHolder NormalizedHolder { get; set; }
    public IStorageSlot NormalizedSlot { get; set; }
}