namespace Bam.Storage;

/// <summary>
/// Reads typed values from storage slots by converting raw byte data to the requested type.
/// </summary>
public interface ISlotReader
{
    /// <summary>
    /// Attempts to read a typed value from the specified storage slot without throwing on failure.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the slot data as.</typeparam>
    /// <param name="slot">The storage slot to read from.</param>
    /// <param name="value">When this method returns, contains the deserialized value if successful, or the default value if not.</param>
    /// <returns><c>true</c> if the slot was read and deserialized successfully; otherwise, <c>false</c>.</returns>
    bool TryReadSlot<T>(IStorageSlot slot, out T value);

    /// <summary>
    /// Reads and deserializes a typed value from the specified storage slot.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the slot data as.</typeparam>
    /// <param name="slot">The storage slot to read from.</param>
    /// <returns>The deserialized value.</returns>
    T ReadSlot<T>(IStorageSlot slot);
}