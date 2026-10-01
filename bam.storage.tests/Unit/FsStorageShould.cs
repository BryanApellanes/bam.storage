using System.Text;
using Bam.Console;
using Bam.Application.TestClasses;
using Bam.DependencyInjection;
using Bam.Services;
using Bam.Storage;
using Bam.Test;

namespace Bam.Application.Unit;

[UnitTestMenu("FsRawDataStorageShould")]
public class FsStorageShould : UnitTestMenuContainer
{
    public FsStorageShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void SaveFile()
    {
        string root = Path.Combine(Environment.CurrentDirectory, nameof(SaveFile));
        ulong testKey = 32.RandomLetters().ToHashULong(HashAlgorithms.SHA256);
        List<string> parts = new List<string>();
        parts.AddRange(typeof(TestStorageData).Namespace!.Split('.'));
        parts.Add(nameof(TestStorageData));
        parts.Add("key");
        parts.AddRange(SegmentedPath.Segments(testKey.ToString()));
        parts.Add(SegmentedPath.DataFileName);

        string relativePath = Path.Combine(parts.ToArray());
        string expected = Path.Combine(root, relativePath);
        string testData = 64.RandomLetters();
        if (File.Exists(expected))
        {
            File.Delete(expected);
        }

        When.A<FsSlottedStorage>("saves a file to the expected path under its root",
            () => new FsSlottedStorage(root),
            (slottedStorage) =>
            {
                slottedStorage.Save(relativePath, new RawData(testData));
                return File.Exists(expected);
            })
        .TheTest
        .ShouldPass<bool>((because, saved) =>
        {
            because.ItsTrue("file was saved", saved);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
