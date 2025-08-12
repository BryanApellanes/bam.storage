using System.Text;
using Bam.Storage;
using Bam.Test;

namespace Bam.Application.Integration;


[UnitTestMenu("FsRawStorageShould")]
public class FsRawStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public async Task SaveAndRetrieveRawData()
    {
        string testData = "this is test data: ".RandomLetters(10);
        FsRawStorage storage = new FsRawStorage($"{nameof(OpaqueFsRawStorageShould)}_{nameof(SaveAndRetrieveRawData)}");
        IRawData data = new RawData(testData);
        IStorageSlot slot = storage.Save(data);

        byte[] value = slot.GetData().Value;
        string retrievedFromSlot = Encoding.UTF8.GetString(value);

        IRawData rawFromStorage = storage.LoadHashHexString(data.HashHexString);
        string retrievedFromStorage = Encoding.UTF8.GetString(rawFromStorage.Value);

        retrievedFromSlot.ShouldNotBeNull();
        retrievedFromSlot.ShouldEqual(testData);

        retrievedFromStorage.ShouldNotBeNull();
        retrievedFromStorage.ShouldEqual(testData);
    }
}