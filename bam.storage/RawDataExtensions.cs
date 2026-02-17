namespace Bam.Storage;

/// <summary>
/// Provides extension methods for <see cref="IRawData"/> instances.
/// </summary>
public static class RawDataExtensions
{
    /// <summary>
    /// Deserializes the raw data from its JSON string representation to an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON data into.</typeparam>
    /// <param name="rawData">The raw data containing JSON content.</param>
    /// <returns>The deserialized object.</returns>
    public static T ToObject<T>(this IRawData rawData)
    {
        return rawData.ToString()!.FromJson<T>();
    }
}