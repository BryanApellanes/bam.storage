namespace Bam.Storage;

public interface ISlotReader
{
    bool TryReadSlot<T>(IStorageSlot slot, out T value);
    T ReadSlot<T>(IStorageSlot slot);
}