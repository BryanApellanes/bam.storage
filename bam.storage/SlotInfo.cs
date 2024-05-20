using Bam.Net;

namespace Bam.Storage;

public class SlotInfo : IStorageSlot
{
    public SlotInfo(IStorageSlot slot)
    {
        this.Original = slot;
        this.FullName = Path.Combine(slot.StorageHolder.FullName, slot.FullName);
        this.Name = Path.GetFileName(this.FullName);
        this.RelativePath = this.Name;
        this.StorageHolder = new HolderInfo(slot.StorageHolder.FullName);
    }

    private IStorageSlot Original { get; }
    public string? FullName { get; }
    public IStorageHolder? StorageHolder { get; }
    public string RelativePath { get; }
    public string Name { get; }
    public IRawData? GetData()
    {
        return this.Original.GetData();
    }

    public void SetData(IRawData rawData)
    {
        this.Original.SetData(rawData);
    }
}