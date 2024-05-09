namespace Bam.Storage;

public class NormalizedSlotInfo
{
    public IStorageContainer Container { get; set; }
    public IStorageSlot Slot { get; set; }
    public IStorageContainer NormalizedContainer { get; set; }
    public IStorageSlot NormalizedSlot { get; set; }
}