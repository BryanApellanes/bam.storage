using Bam.Storage;
using Bam.Test;

namespace Bam.Application.Unit;

/// <summary>
/// Pins bam.storage#1: a segmented path built from a base64 key contains a segment that starts with '/', which
/// made <see cref="Path.Combine(string[])"/> restart at the drive root and write slots to C:\I\…, C:\+\… and so on.
/// Also pins that the guard decides the same way on every platform (fixed character set, reserved device names,
/// trailing dot or space) and that a path is refused by where it resolves as well as by its text.
/// </summary>
[UnitTestMenu("StoragePathGuardShould")]
public class StoragePathGuardShould : UnitTestMenuContainer
{
    [UnitTest]
    public void RejectASegmentedPathWhoseSegmentStartsWithASeparator()
    {
        string base64Key = "ab/Icd+ef==";
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectASegmentedPathWhoseSegmentStartsWithASeparator)));

        When.A<FsStorageHolder>("refuses to build a segmented slot from a base64 key",
            () => holder,
            (rootHolder) => Attempt(() => StorageSlot.GetSegmentedPathStorageSlot(rootHolder, base64Key)))
        .TheTest
        .ShouldPass<AttemptOutcome>((because, outcome) =>
        {
            because.ItsTrue("an ArgumentException was thrown", outcome.Threw);
            because.ItsTrue("the message names the offending segment", outcome.Message.Contains("/I"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void KeepAHexSegmentedPathUnderTheRoot()
    {
        string hashHexString = "0123456789abcdef";
        string root = TestRoot(nameof(KeepAHexSegmentedPathUnderTheRoot));
        string expected = Path.Combine(root, "01", "23", "45", "67", "89", "ab", "cd", "ef", SegmentedPath.DataFileName);

        When.A<FsStorageHolder>("builds the segmented slot inside its root",
            () => new FsStorageHolder(root),
            (rootHolder) => StorageSlot.GetSegmentedPathStorageSlot(rootHolder, hashHexString).FullName!)
        .TheTest
        .ShouldPass<string>((because, fullName) =>
        {
            because.ItsTrue("the slot path is the expected segmented path", expected.Equals(fullName));
            because.ItsTrue("the slot path starts with the root", fullName.StartsWith(root));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void KeepTheRemainderOfAnOddLengthIdentifier()
    {
        string expected = Path.Combine("ab", "cd", "e", SegmentedPath.DataFileName);

        When.A<string>("keeps the trailing character as its own segment",
            () => "abcde",
            (identifier) => SegmentedPath.FromHashHexString(identifier))
        .TheTest
        .ShouldPass<string>((because, relativePath) =>
        {
            because.ItsTrue("the remainder is not dropped", expected.Equals(relativePath));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectARootedRelativePath()
    {
        string rooted = Path.GetFullPath(SegmentedPath.DataFileName);
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectARootedRelativePath)));

        When.A<FsStorageHolder>("refuses a slot whose relative path is rooted",
            () => holder,
            (rootHolder) => Attempt(() => new FsStorageSlot(rootHolder, rooted)))
        .TheTest
        .ShouldPass<AttemptOutcome>((because, outcome) =>
        {
            because.ItsTrue("an ArgumentException was thrown", outcome.Threw);
            because.ItsTrue("the message says the path must be relative", outcome.Message.Contains("must be relative"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectParentDirectoryNavigation()
    {
        string traversal = Path.Combine("..", "escaped", SegmentedPath.DataFileName);
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectParentDirectoryNavigation)));

        When.A<FsStorageHolder>("refuses a slot whose relative path navigates to the parent",
            () => holder,
            (rootHolder) => Attempt(() => new FsStorageSlot(rootHolder, traversal)))
        .TheTest
        .ShouldPass<AttemptOutcome>((because, outcome) =>
        {
            because.ItsTrue("an ArgumentException was thrown", outcome.Threw);
            because.ItsTrue("the message names the navigation segment", outcome.Message.Contains("'..'"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void AcceptAnOrdinaryRelativePath()
    {
        string relativePath = Path.Combine("profile", "settings", SegmentedPath.DataFileName);
        string root = TestRoot(nameof(AcceptAnOrdinaryRelativePath));

        When.A<FsStorageHolder>("accepts a plain relative path",
            () => new FsStorageHolder(root),
            (rootHolder) => new FsStorageSlot(rootHolder, relativePath).FullName!)
        .TheTest
        .ShouldPass<string>((because, fullName) =>
        {
            because.ItsTrue("the slot resolves under the root", Path.Combine(root, relativePath).Equals(fullName));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectCharactersThatAreInvalidOnAnyPlatform()
    {
        // Path.GetInvalidFileNameChars() reports only NUL and '/' on Unix; the guard must not depend on it.
        string[] hostile = new string[] { "a?b", "a*b", "a:b", "a\"b", "a<b", "a>b", "a|b", "a\tb", "a\nb", "a\u001fb", "a\0b" };
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectCharactersThatAreInvalidOnAnyPlatform)));

        When.A<FsStorageHolder>("refuses a segment containing a character Windows cannot store",
            () => holder,
            (rootHolder) => AcceptedBy(rootHolder, hostile.Select(name => $"reports/{name}")))
        .TheTest
        .ShouldPass<List<string>>((because, accepted) =>
        {
            because.ItsTrue("every path was refused", accepted.Count == 0, $"accepted: {Printable(accepted)}");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectWindowsDeviceNames()
    {
        // 'reports/NUL' resolved to \\.\NUL on Windows and SetData died with a NullReferenceException.
        string[] deviceNames = new string[] { "NUL", "reports/NUL", "reports/nul", "reports/NUL.txt", "reports/nul.tar.gz", "CON", "prn", "Aux", "COM1", "lpt9", "COM\u00B9", "CONIN$", "reports/CONOUT$" };
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectWindowsDeviceNames)));

        When.A<FsStorageHolder>("refuses a segment that names a Windows device",
            () => holder,
            (rootHolder) => AcceptedBy(rootHolder, deviceNames))
        .TheTest
        .ShouldPass<List<string>>((because, accepted) =>
        {
            because.ItsTrue("every device name was refused", accepted.Count == 0, $"accepted: {Printable(accepted)}");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectSegmentsThatEndWithADotOrASpace()
    {
        // Windows strips a trailing '.' or ' ', so 'a./b' and 'a/b' are one directory there and two on Unix.
        string[] aliases = new string[] { "a./b", "a /b", "a/b.", "a/b ", "a/...", "a/.. ", "a/b. ." };
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectSegmentsThatEndWithADotOrASpace)));

        When.A<FsStorageHolder>("refuses a segment Windows would rename",
            () => holder,
            (rootHolder) =>
            {
                List<string> accepted = AcceptedBy(rootHolder, aliases);
                AttemptOutcome segmented = Attempt(() => StorageSlot.GetSegmentedPathStorageSlot(rootHolder, "a key"));
                return new AliasOutcome(accepted, segmented.Threw);
            })
        .TheTest
        .ShouldPass<AliasOutcome>((because, outcome) =>
        {
            because.ItsTrue("every aliasing path was refused", outcome.Accepted.Count == 0, $"accepted: {Printable(outcome.Accepted)}");
            because.ItsTrue("a segmented identifier with a chunk ending in a space was refused", outcome.SegmentedIdentifierRefused);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectEveryRootedOrNavigatingShape()
    {
        string[] hostile = new string[]
        {
            "a/../x", "a\\..\\..\\b", "a/..", "./a", "a/./b", ".",
            "/x", "\\x", "C:x", "C:\\x", "C:/x", "\\\\server\\share\\x", "//server/share/x", "\\\\?\\C:\\x",
            "a/b:stream", "a/b::$DATA"
        };
        FsStorageHolder holder = new FsStorageHolder(TestRoot(nameof(RejectEveryRootedOrNavigatingShape)));

        When.A<FsStorageHolder>("refuses rooted, drive-relative, UNC, stream and navigating paths in any position",
            () => holder,
            (rootHolder) => AcceptedBy(rootHolder, hostile))
        .TheTest
        .ShouldPass<List<string>>((because, accepted) =>
        {
            because.ItsTrue("every path was refused", accepted.Count == 0, $"accepted: {Printable(accepted)}");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void AcceptNamesThatOnlyResembleRefusedOnes()
    {
        string[] ordinary = new string[] { "console/dat", "nulls", "auxiliary.json", "com10", "lpt", ".hidden", "a.b/c", "a b/c d", "..a", "reports/2026-10-01" };
        string root = TestRoot(nameof(AcceptNamesThatOnlyResembleRefusedOnes));

        When.A<FsStorageHolder>("accepts ordinary names and resolves them under its root",
            () => new FsStorageHolder(root),
            (rootHolder) =>
            {
                List<string> accepted = AcceptedBy(rootHolder, ordinary);
                List<string> outsideRoot = accepted
                    .Where(name => !Path.GetFullPath(new FsStorageSlot(rootHolder, name).FullName!).StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                    .ToList();
                return new AcceptanceOutcome(accepted.Count, outsideRoot);
            })
        .TheTest
        .ShouldPass<AcceptanceOutcome>((because, outcome) =>
        {
            because.ItsTrue("every ordinary name was accepted", outcome.AcceptedCount == ordinary.Length);
            because.ItsTrue("every accepted name resolves under the root", outcome.OutsideRoot.Count == 0, $"outside the root: {Printable(outcome.OutsideRoot)}");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectAPathThatResolvesOutsideTheHolder()
    {
        // Calls the resolved-path check directly: the paths below are refused by where they land, not by their text.
        string root = Path.Combine(TestRoot(nameof(RejectAPathThatResolvesOutsideTheHolder)), "data");
        string intoParent = Path.Combine("..", "escaped", SegmentedPath.DataFileName);
        string intoSibling = Path.Combine("..", "data2", SegmentedPath.DataFileName);
        string inside = Path.Combine("reports", SegmentedPath.DataFileName);

        When.A<string>("refuses a relative path whose full path leaves the holder",
            () => root,
            (holderFullName) => new ContainmentOutcome(
                Attempt(() => StoragePathGuard.EnsureContained(holderFullName, intoParent, "relativePath")),
                Attempt(() => StoragePathGuard.EnsureContained(holderFullName, intoSibling, "relativePath")),
                Attempt(() => StoragePathGuard.EnsureContained(holderFullName, "reports/NUL", "relativePath")),
                Attempt(() => StoragePathGuard.EnsureContained(holderFullName + Path.DirectorySeparatorChar, inside, "relativePath")),
                Attempt(() => StoragePathGuard.EnsureContained(holderFullName, inside, "relativePath"))))
        .TheTest
        .ShouldPass<ContainmentOutcome>((because, outcome) =>
        {
            because.ItsTrue("a path into the parent was refused", outcome.IntoParent.Threw);
            because.ItsTrue("the message says where the path resolved", outcome.IntoParent.Message.Contains("resolves outside the storage holder"));
            because.ItsTrue("a sibling whose name starts with the holder's name was refused", outcome.IntoSibling.Threw);
            // Only Windows resolves NUL to a device; on Unix it is an ordinary file name inside the holder.
            because.ItsTrue("a device name was refused where the platform resolves it to a device", outcome.DeviceName.Threw == OperatingSystem.IsWindows());
            because.ItsTrue("a path inside a holder written with a trailing separator was accepted", !outcome.InsideWithTrailingSeparator.Threw);
            because.ItsTrue("a path inside the holder was accepted", !outcome.Inside.Threw);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static string TestRoot(string testName)
    {
        return Path.Combine(Environment.CurrentDirectory, nameof(StoragePathGuardShould), testName);
    }

    private static List<string> AcceptedBy(IStorageHolder holder, IEnumerable<string> relativePaths)
    {
        List<string> accepted = new List<string>();
        foreach (string relativePath in relativePaths)
        {
            if (!Attempt(() => new FsStorageSlot(holder, relativePath)).Threw)
            {
                accepted.Add(relativePath);
            }
        }
        return accepted;
    }

    private static string Printable(IEnumerable<string> values)
    {
        return string.Join(", ", values.Select(value => $"'{string.Concat(value.Select(c => char.IsControl(c) ? $"\\u{(int)c:x4}" : c.ToString()))}'"));
    }

    private static AttemptOutcome Attempt(Action action)
    {
        try
        {
            action();
            return new AttemptOutcome(false, string.Empty);
        }
        catch (ArgumentException ex)
        {
            return new AttemptOutcome(true, ex.Message);
        }
    }

    private sealed record AttemptOutcome(bool Threw, string Message);

    private sealed record AliasOutcome(List<string> Accepted, bool SegmentedIdentifierRefused);

    private sealed record AcceptanceOutcome(int AcceptedCount, List<string> OutsideRoot);

    private sealed record ContainmentOutcome(AttemptOutcome IntoParent, AttemptOutcome IntoSibling, AttemptOutcome DeviceName, AttemptOutcome InsideWithTrailingSeparator, AttemptOutcome Inside);
}
