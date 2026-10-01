using Bam.Storage;
using Bam.Test;

namespace Bam.Application.Unit;

/// <summary>
/// Pins bam.storage#1: a segmented path built from a base64 key contains a segment that starts with '/', which
/// made <see cref="Path.Combine(string[])"/> restart at the drive root and write slots to C:\I\…, C:\+\… and so on.
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

    private static string TestRoot(string testName)
    {
        return Path.Combine(Environment.CurrentDirectory, nameof(StoragePathGuardShould), testName);
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
}
