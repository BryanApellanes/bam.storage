using System.Text;
using Bam.Console;
using Bam.Net.Application.TestClasses;
using Bam.Net.CoreServices;
using Bam.Storage;
using Bam.Testing;

namespace Bam.Net.Application.Unit;

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
        string storagePath = storage.GetStoragePath(rawData);
        string idPath = storage.GetHashIdPath(rawData.HashId);
        string hashPath = storage.GetHashPath(rawData.HashString);
        
        Message.PrintLine(hashPath);
        idPath.ShouldBeEqualTo(hashPath);
        storagePath.ShouldBeEqualTo(idPath);
    }
    
    private void DeleteFileIfItExists(string file)
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }
}