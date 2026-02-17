using Bam.Data;

namespace Bam.Storage;

/// <summary>
/// Reads typed values from file-system storage slots by converting raw byte data to the requested
/// CLR type using <see cref="BitConverter"/> for primitives and parsing for date/string types.
/// </summary>
public class FsSlotReader : ISlotReader
{
    /// <summary>
    /// Initializes a new instance of <see cref="FsSlotReader"/> using the default data type translator.
    /// </summary>
    public FsSlotReader()
    {
        this.DataTypeTranslator = Bam.Storage.Data.DataTypeTranslator.Default;
    }

    /// <summary>
    /// Gets the data type translator used to determine the CLR type from the requested generic type parameter.
    /// </summary>
    protected IDataTypeTranslator DataTypeTranslator { get; }

    /// <summary>
    /// Attempts to read a typed value from the specified storage slot without throwing on failure.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the slot data as.</typeparam>
    /// <param name="slot">The storage slot to read from.</param>
    /// <param name="value">When this method returns, contains the deserialized value if successful, or the default value if not.</param>
    /// <returns><c>true</c> if the slot was read and deserialized successfully; otherwise, <c>false</c>.</returns>
    public bool TryReadSlot<T>(IStorageSlot slot, out T value)
    {
        value = default!;
        try
        {
            value = ReadSlot<T>(slot);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Reads and deserializes a typed value from the specified storage slot.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the slot data as.</typeparam>
    /// <param name="slot">The storage slot to read from.</param>
    /// <returns>The deserialized value of type <typeparamref name="T"/>.</returns>
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
                return ReadString(slot)!;
            case DataTypes.Boolean:
                return ReadBoolean(slot)!;
            case DataTypes.Int:
                return ReadInt(slot)!;
            case DataTypes.UInt:
                return ReadUInt(slot)!;
            case DataTypes.ULong:
                return ReadULong(slot)!;
            case DataTypes.Long:
                return ReadLong(slot)!;
            case DataTypes.Decimal:
                return ReadDecimal(slot)!;
            case DataTypes.String:
                return ReadString(slot)!;
            case DataTypes.ByteArray:
                return slot.GetData()!.Value;
            case DataTypes.DateTime:
                return ReadDateTime(slot)!;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string? ReadString(IStorageSlot slot)
    {
        return BitConverter.ToString(slot.GetData()!.Value);
    }

    private bool? ReadBoolean(IStorageSlot slot)
    {
        return BitConverter.ToBoolean(slot.GetData()!.Value);
    }

    private int? ReadInt(IStorageSlot slot)
    {
        return BitConverter.ToInt32(slot.GetData()!.Value);
    }

    private uint? ReadUInt(IStorageSlot slot)
    {
        return BitConverter.ToUInt32(slot.GetData()!.Value);
    }

    private ulong? ReadULong(IStorageSlot slot)
    {
        return BitConverter.ToUInt64(slot.GetData()!.Value);
    }

    private long? ReadLong(IStorageSlot slot)
    {
        return BitConverter.ToInt64(slot.GetData()!.Value);
    }

    private decimal? ReadDecimal(IStorageSlot slot)
    {
        byte[] bytes = slot.GetData()!.Value;
        int[] bits = new int[4];
        bits[0] = ((bytes[0] | (bytes[1] << 8)) | (bytes[2] << 0x10)) | (bytes[3] << 0x18); //lo
        bits[1] = ((bytes[4] | (bytes[5] << 8)) | (bytes[6] << 0x10)) | (bytes[7] << 0x18); //mid
        bits[2] = ((bytes[8] | (bytes[9] << 8)) | (bytes[10] << 0x10)) | (bytes[11] << 0x18); //hi
        bits[3] = ((bytes[12] | (bytes[13] << 8)) | (bytes[14] << 0x10)) | (bytes[15] << 0x18); //flags

        return new decimal(bits);
    }

    private DateTime? ReadDateTime(IStorageSlot slot)
    {
        return DateTime.Parse(ReadString(slot)!);
    }
}