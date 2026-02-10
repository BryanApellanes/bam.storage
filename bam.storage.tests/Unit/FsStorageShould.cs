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
        List<string> parts = new List<string> { root };
        parts.AddRange(typeof(TestStorageData).Namespace.Split('.'));
        parts.Add(nameof(TestStorageData));
        parts.Add("key");
        parts.AddRange(testKey.ToString().Split(2));
        parts.Add("dat");

        string expected = Path.Combine(parts.ToArray());
        string testData = 64.RandomLetters();
        if (File.Exists(expected))
        {
            File.Delete(expected);
        }

        When.A<FsSlottedStorage>("saves a file to the expected path",
            () => new FsSlottedStorage(expected),
            (slottedStorage) =>
            {
                slottedStorage.Save(expected, new RawData(testData));
                return File.Exists(expected);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("file was saved", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
