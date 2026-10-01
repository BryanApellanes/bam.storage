namespace Bam.Storage;

/// <summary>
/// Validates the relative paths and path segments that storage slots are built from, so that no key, hash or
/// caller-supplied name can ever resolve to a location outside the storage holder's root.
/// </summary>
/// <remarks>
/// <see cref="Path.Combine(string[])"/> restarts at any argument that is rooted. A segment such as <c>/I</c>
/// (the kind standard base64 produces) therefore discards every preceding part and the slot is written to the
/// drive root. The checks here are deliberately platform-independent: a key that is rejected on Linux is rejected
/// identically on Windows and vice versa, so the on-disk layout never depends on where the key was generated.
/// </remarks>
public static class StoragePathGuard
{
    private static readonly char[] SeparatorChars = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, '/', '\\' };
    private static readonly char[] InvalidSegmentChars = BuildInvalidSegmentChars();

    /// <summary>
    /// Ensures the specified path is relative, contains no navigation segments (<c>.</c> or <c>..</c>) and no
    /// characters that are invalid in a path segment on any supported platform.
    /// </summary>
    /// <param name="relativePath">The path to validate, relative to a storage holder.</param>
    /// <param name="paramName">The caller's parameter name, used in the exception message.</param>
    /// <returns>The validated <paramref name="relativePath"/>, unchanged.</returns>
    /// <exception cref="ArgumentException">The path is null, empty, rooted, or contains an unsafe segment.</exception>
    public static string EnsureRelativePath(string relativePath, string paramName)
    {
        Args.ThrowIfNullOrEmpty(relativePath, paramName);
        if (Path.IsPathRooted(relativePath) || relativePath[0] == '/' || relativePath[0] == '\\')
        {
            throw new ArgumentException($"{paramName} must be relative to the storage holder; got a rooted path: {relativePath}", paramName);
        }

        string[] segments = relativePath.Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            throw new ArgumentException($"{paramName} has no path segments: {relativePath}", paramName);
        }

        foreach (string segment in segments)
        {
            EnsureSafeSegment(segment, paramName);
        }

        return relativePath;
    }

    /// <summary>
    /// Ensures a single path segment is non-empty, is not <c>.</c> or <c>..</c>, and contains no directory separator,
    /// volume separator or other character that is invalid in a file name on any supported platform.
    /// </summary>
    /// <param name="segment">The segment to validate.</param>
    /// <param name="paramName">The caller's parameter name, used in the exception message.</param>
    /// <returns>The validated <paramref name="segment"/>, unchanged.</returns>
    /// <exception cref="ArgumentException">The segment is empty, a navigation segment, or contains an unsafe character.</exception>
    public static string EnsureSafeSegment(string segment, string paramName)
    {
        if (string.IsNullOrEmpty(segment))
        {
            throw new ArgumentException($"{paramName} contains an empty path segment", paramName);
        }

        if (segment == "." || segment == "..")
        {
            throw new ArgumentException($"{paramName} may not contain the navigation segment '{segment}'", paramName);
        }

        int invalidIndex = segment.IndexOfAny(InvalidSegmentChars);
        if (invalidIndex >= 0)
        {
            throw new ArgumentException($"{paramName} contains '{segment[invalidIndex]}', which is not valid in a storage path segment: {segment}", paramName);
        }

        return segment;
    }

    private static char[] BuildInvalidSegmentChars()
    {
        HashSet<char> invalid = new HashSet<char>(Path.GetInvalidFileNameChars());
        invalid.Add(Path.DirectorySeparatorChar);
        invalid.Add(Path.AltDirectorySeparatorChar);
        invalid.Add(Path.VolumeSeparatorChar);
        invalid.Add('/');
        invalid.Add('\\');
        invalid.Add(':');
        return invalid.ToArray();
    }
}
