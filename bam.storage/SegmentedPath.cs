namespace Bam.Storage;

/// <summary>
/// Builds the segmented relative path used by content-addressed storage slots: the identifying string is split
/// into two-character segments that become nested directories, followed by the data file name.
/// For example, <c>"abcdef"</c> produces <c>"ab/cd/ef/dat"</c> and <c>"abcde"</c> produces <c>"ab/cd/e/dat"</c>.
/// </summary>
/// <remarks>
/// Every segment is validated by <see cref="StoragePathGuard.EnsureSafeSegment"/>, so an identifier that contains a
/// directory separator (standard base64, for instance) is rejected instead of silently escaping the storage root.
/// The trailing remainder of an odd-length identifier is kept as its own segment; dropping it would make two
/// identifiers that differ only in their last character share a slot.
/// </remarks>
public static class SegmentedPath
{
    /// <summary>
    /// The file name of the data file at the end of every segmented path.
    /// </summary>
    public const string DataFileName = "dat";

    /// <summary>
    /// The number of characters in each directory segment.
    /// </summary>
    public const int SegmentLength = 2;

    /// <summary>
    /// Builds the segmented relative path for the specified identifier.
    /// </summary>
    /// <param name="hashHexString">The identifying string, conventionally a hex-encoded hash.</param>
    /// <returns>A relative path of the form <c>"ab/cd/.../dat"</c>.</returns>
    /// <exception cref="ArgumentException">The identifier is null or empty, or contains a character that is not safe in a path segment.</exception>
    public static string FromHashHexString(string hashHexString)
    {
        Args.ThrowIfNullOrEmpty(hashHexString, nameof(hashHexString));

        List<string> parts = new List<string>();
        foreach (string segment in Segments(hashHexString))
        {
            parts.Add(StoragePathGuard.EnsureSafeSegment(segment, nameof(hashHexString)));
        }
        parts.Add(DataFileName);
        return Path.Combine(parts.ToArray());
    }

    /// <summary>
    /// Splits the specified value into consecutive segments of <see cref="SegmentLength"/> characters, keeping any
    /// shorter remainder as the final segment.
    /// </summary>
    /// <param name="value">The value to split.</param>
    /// <returns>The segments in order.</returns>
    public static IEnumerable<string> Segments(string value)
    {
        for (int index = 0; index < value.Length; index += SegmentLength)
        {
            int length = Math.Min(SegmentLength, value.Length - index);
            yield return value.Substring(index, length);
        }
    }
}
