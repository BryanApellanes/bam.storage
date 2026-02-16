namespace Bam.Storage;

/// <summary>
/// Provides storage operations for key-value pairs, supporting save and retrieval by key.
/// </summary>
public interface IKeyValuePairStorage
{
    /// <summary>
    /// Saves a key-value pair using string representations for both key and value.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The string value to store (will be encoded to bytes).</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    IKeyValuePairSaveResult Save(string key, string value);

    /// <summary>
    /// Saves the specified key-value pair to storage.
    /// </summary>
    /// <param name="keyValuePair">The key-value pair to save.</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair);

    /// <summary>
    /// Retrieves the key-value pair associated with the specified key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The key-value pair associated with the specified key.</returns>
    IKeyValuePair Get(string key);
}