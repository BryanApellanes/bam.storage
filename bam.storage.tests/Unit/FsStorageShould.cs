using System.Text;
using Bam.Console;
using Bam.Application.TestClasses;
using Bam.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Application.Unit;

[UnitTestMenu("FsRawDataStorageShould")]
public class FsStorageShould : UnitTestMenuContainer
{
    public FsStorageShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void ReturnSamePathForHashes()
    {
        FsStorage storage = Get<FsStorage>();
        string value = 256.RandomLetters();
        byte[] valueBytes = Encoding.UTF8.GetBytes(value);
        RawData rawData = new RawData(valueBytes);
        string storagePath = storage.GetHashLongIdStoragePath(rawData);
        string idPath = storage.GetHashLongIdStorageSlotPath(rawData.HashId);
        string hashPath = storage.GetHashLongIdPathFromHashHexString(rawData.HashHexString);
        
        Message.PrintLine(hashPath);
        idPath.ShouldBeEqualTo(hashPath);
        storagePath.ShouldBeEqualTo(idPath);
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
        IStorage storage = new FsStorage(expected);
        storage.Save(expected, new RawData(testData));
        File.Exists(expected).ShouldBeTrue("file was not saved");
    }
    
    private void DeleteFileIfItExists(string file)
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }
}