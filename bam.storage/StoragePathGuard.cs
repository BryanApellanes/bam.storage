namespace Bam.Storage;

/// <summary>
/// Validates the relative paths and path segments that storage slots are built from, so that no key, hash or
/// caller-supplied name can resolve to a location outside the storage holder's root.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Path.Combine(string[])"/> restarts at any argument that is rooted. A segment such as <c>/I</c>
/// (the kind standard base64 produces) therefore discards every preceding part and the slot is written to the
/// drive root.
/// </para>
/// <para>
/// The syntactic checks (<see cref="EnsureRelativePath"/> and <see cref="EnsureSafeSegment"/>) use a fixed character
/// set and a fixed list of reserved names instead of what the current runtime reports as invalid
/// (<see cref="Path.GetInvalidFileNameChars"/> returns 41 characters on Windows and 2 on Unix). A path is therefore
/// accepted or rejected the same way on every platform, and a store written on Linux never holds an entry that
/// Windows cannot address. The Windows rules are used because they are the superset.
/// </para>
/// <para>
/// <see cref="EnsureContained"/> is the backstop. It resolves the path with the current platform's own rules and
/// refuses anything that lands outside the holder, which covers whatever the syntactic checks did not anticipate.
/// It depends on the platform by nature.
/// </para>
/// <para>
/// One platform difference remains. <c>\</c> separates directories on Windows and is an ordinary file-name character
/// on Unix. Both separators are split on here, so neither can hide a navigation segment, but a relative path written
/// with <c>\</c> names a nested path on Windows and a single file on Unix. Build relative paths with
/// <see cref="Path.Combine(string[])"/> or <c>/</c>.
/// </para>
/// </remarks>
public static class StoragePathGuard
{
    private static readonly char[] SeparatorChars = new char[] { '/', '\\' };
    private static readonly char[] InvalidSegmentChars = BuildInvalidSegmentChars();
    private static readonly HashSet<string> ReservedDeviceNames = BuildReservedDeviceNames();

    /// <summary>
    /// Ensures the specified path is relative, contains no navigation segments (<c>.</c> or <c>..</c>) and that every
    /// segment passes <see cref="EnsureSafeSegment"/>.
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
    /// Ensures a single path segment is safe to use as a directory or file name on every supported platform.
    /// </summary>
    /// <remarks>
    /// A segment is refused when it is empty; is <c>.</c> or <c>..</c>; contains a directory separator, a volume
    /// separator, a control character or one of <c>" &lt; &gt; | * ?</c>; ends with <c>.</c> or a space (Windows
    /// strips both, so <c>a.</c> and <c>a</c> would be the same directory there and different ones on Unix); or is a
    /// Windows reserved device name such as <c>NUL</c> or <c>COM1</c>, with or without an extension (Windows resolves
    /// those to a device instead of a file under the holder).
    /// </remarks>
    /// <param name="segment">The segment to validate.</param>
    /// <param name="paramName">The caller's parameter name, used in the exception message.</param>
    /// <returns>The validated <paramref name="segment"/>, unchanged.</returns>
    /// <exception cref="ArgumentException">The segment is empty, a navigation segment, a reserved device name, or contains or ends with an unsafe character.</exception>
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
            throw new ArgumentException($"{paramName} contains {Describe(segment[invalidIndex])}, which is not valid in a storage path segment: {segment}", paramName);
        }

        char last = segment[segment.Length - 1];
        if (last == '.' || last == ' ')
        {
            throw new ArgumentException($"{paramName} contains a segment that ends with {Describe(last)}, which Windows strips, so it would not name the same location on every platform: '{segment}'", paramName);
        }

        if (IsReservedDeviceName(segment))
        {
            throw new ArgumentException($"{paramName} contains the reserved device name '{segment}', which Windows resolves to a device instead of a location in the storage holder", paramName);
        }

        return segment;
    }

    /// <summary>
    /// Ensures the specified relative path, resolved against the holder's path by the current platform, stays inside
    /// the holder.
    /// </summary>
    /// <remarks>
    /// This is the resolved-path counterpart of <see cref="EnsureRelativePath"/>: that method judges the text of the
    /// path, this one judges where the operating system would actually put it. The resolved path must begin with the
    /// holder's full path followed by a directory separator, so a sibling such as <c>data2</c> is not mistaken for a
    /// child of <c>data</c>. Links (symbolic links, junctions) are not followed.
    /// </remarks>
    /// <param name="holderFullName">The full path of the storage holder the slot belongs to.</param>
    /// <param name="relativePath">The slot's path, relative to the holder.</param>
    /// <param name="paramName">The caller's parameter name, used in the exception message.</param>
    /// <returns>The validated <paramref name="relativePath"/>, unchanged.</returns>
    /// <exception cref="ArgumentException">The holder path or the relative path is null or empty, or the path resolves to a location outside the holder.</exception>
    public static string EnsureContained(string holderFullName, string relativePath, string paramName)
    {
        Args.ThrowIfNullOrEmpty(holderFullName, nameof(holderFullName));
        Args.ThrowIfNullOrEmpty(relativePath, paramName);

        string root = Path.GetFullPath(holderFullName);
        string rootWithSeparator = Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar;
        string resolved = Path.GetFullPath(Path.Combine(root, relativePath));
        if (!resolved.StartsWith(rootWithSeparator, StringComparison.Ordinal))
        {
            throw new ArgumentException($"{paramName} resolves outside the storage holder: '{relativePath}' resolves to {resolved}, which is not under {root}", paramName);
        }

        return relativePath;
    }

    private static bool IsReservedDeviceName(string segment)
    {
        int extensionIndex = segment.IndexOf('.');
        string baseName = extensionIndex < 0 ? segment : segment.Substring(0, extensionIndex);
        return ReservedDeviceNames.Contains(baseName.TrimEnd(' '));
    }

    private static string Describe(char value)
    {
        if (value == ' ')
        {
            return "a space";
        }

        return char.IsControl(value) ? $"the control character U+{(int)value:X4}" : $"'{value}'";
    }

    private static char[] BuildInvalidSegmentChars()
    {
        List<char> invalid = new List<char>();
        for (char control = '\0'; control < ' '; control++)
        {
            invalid.Add(control);
        }

        invalid.AddRange(new char[] { '"', '<', '>', '|', ':', '*', '?', '\\', '/' });
        return invalid.ToArray();
    }

    private static HashSet<string> BuildReservedDeviceNames()
    {
        HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CON", "PRN", "AUX", "NUL", "CONIN$", "CONOUT$" };
        foreach (string port in new string[] { "COM", "LPT" })
        {
            foreach (char suffix in "0123456789¹²³")
            {
                names.Add($"{port}{suffix}");
            }
        }

        return names;
    }
}
