using System.Text;
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Integration;


[UnitTestMenu("OpaqueFsRawStorageShould")]
public class OpaqueFsRawStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public async Task SaveAndRetrieveRawData()
    {
        string testData = "this is test data";
        AesKey aesKey = new AesKey();
        OpaqueFsRawStorage storage = new OpaqueFsRawStorage(aesKey, new HmacKeyProvider(), $"{nameof(OpaqueFsRawStorageShould)}_{nameof(SaveAndRetrieveRawData)}");
        IRawData data = new RawData(testData);
        IStorageSlot slot = storage.Save(data);

        //byte[] value = slot.GetData().Value;
        //string retrievedFromSlot = Encoding.UTF8.GetString(value);

        IRawData rawFromStorage = storage.LoadHashHexString(data.HashHexString);
        string retrievedFromStorage = Encoding.UTF8.GetString(rawFromStorage.Value);

        //retrievedFromSlot.ShouldNotBeNull();
        //retrievedFromSlot.ShouldEqual(testData);

        retrievedFromStorage.ShouldNotBeNull();
        retrievedFromStorage.ShouldEqual(testData);
    }
}