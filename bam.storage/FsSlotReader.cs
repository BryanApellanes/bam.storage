using Bam.Data;
using Bam.Net;
using Bam.Net.Data;

namespace Bam.Storage;

public class FsSlotReader : ISlotReader
{
    public FsSlotReader()
    {
        this.DataTypeTranslator = Bam.Data.DataTypeTranslator.Default;
    }
    
    protected IDataTypeTranslator DataTypeTranslator { get; }

    public bool TryReadSlot<T>(IStorageSlot slot, out T value)
    {
        value = default;
        try
        {
            value = ReadSlot<T>(slot);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public T ReadSlot<T>(IStorageSlot slot)
    {
        return (T)ReadSlot(slot, typeof(T));
    }
    
    private object ReadSlot(IStorageSlot slot, Type type)
    {
        Args.ThrowIfNull(slot, "slot");
        DataTypes dataType = DataTypeTranslator.EnumFromType(type);
        switch (dataType)
        {
            case DataTypes.Default:
                return ReadString(slot);
                break;
            case DataTypes.Boolean:
                return ReadBoolean(slot);
                break;
            case DataTypes.Int:
                return ReadInt(slot);
                break;
            case DataTypes.UInt:
                return ReadUInt(slot);
                break;
            case DataTypes.ULong:
                return ReadULong(slot);
                break;
            case DataTypes.Long:
                return ReadLong(slot);
                break;
            case DataTypes.Decimal:
                return ReadDecimal(slot);
                break;
            case DataTypes.String:
                return ReadString(slot);
                break;
            case DataTypes.ByteArray:
                return slot.GetData().Value;
                break;
            case DataTypes.DateTime:
                return ReadDateTime(slot);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string? ReadString(IStorageSlot slot)
    {
        return BitConverter.ToString(slot.GetData().Value);
    }
    
    private bool? ReadBoolean(IStorageSlot slot)
    {
        return BitConverter.ToBoolean(slot.GetData().Value);
    }
    
    private int? ReadInt(IStorageSlot slot)
    {
        return BitConverter.ToInt32(slot.GetData().Value);
    }
    
    private uint? ReadUInt(IStorageSlot slot)
    {
        return BitConverter.ToUInt32(slot.GetData().Value);
    }

    private ulong? ReadULong(IStorageSlot slot)
    {
        return BitConverter.ToUInt64(slot.GetData().Value);
    }
    
    private long? ReadLong(IStorageSlot slot)
    {
        return BitConverter.ToInt64(slot.GetData().Value);
    }

    private decimal? ReadDecimal(IStorageSlot slot)
    {
        byte[] bytes = slot.GetData().Value;
        int[] bits = new int[4];
        bits[0] = ((bytes[0] | (bytes[1] << 8)) | (bytes[2] << 0x10)) | (bytes[3] << 0x18); //lo
        bits[1] = ((bytes[4] | (bytes[5] << 8)) | (bytes[6] << 0x10)) | (bytes[7] << 0x18); //mid
        bits[2] = ((bytes[8] | (bytes[9] << 8)) | (bytes[10] << 0x10)) | (bytes[11] << 0x18); //hi
        bits[3] = ((bytes[12] | (bytes[13] << 8)) | (bytes[14] << 0x10)) | (bytes[15] << 0x18); //flags

        return new decimal(bits);
    }

    private DateTime? ReadDateTime(IStorageSlot slot)
    {
        return DateTime.Parse(ReadString(slot));
    }
}