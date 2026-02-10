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
    public void SaveAndRetrieveRawData()
    {
        string testData = "this is test data";

        When.A<OpaqueFsRawStorage>("saves and retrieves encrypted raw data",
            () => new OpaqueFsRawStorage(new AesKey(), new HmacKeyProvider(), $"{nameof(OpaqueFsRawStorageShould)}_{nameof(SaveAndRetrieveRawData)}"),
            (storage) =>
            {
                IRawData data = new RawData(testData);
                IStorageSlot slot = storage.Save(data);
                IRawData rawFromStorage = storage.LoadHashHexString(data.HashHexString);
                string retrievedFromStorage = Encoding.UTF8.GetString(rawFromStorage.Value);
                return retrievedFromStorage;
            })
        .TheTest
        .ShouldPass(because =>
        {
            string retrieved = (string)because.Result;
            because.ItsTrue("retrieved is not null", retrieved != null);
            because.ItsTrue("retrieved equals original", testData.Equals(retrieved));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
