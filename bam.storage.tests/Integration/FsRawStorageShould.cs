using System.Text;
using Bam.Storage;
using Bam.Test;

namespace Bam.Application.Integration;

[UnitTestMenu("FsRawStorageShould")]
public class FsRawStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public void SaveAndRetrieveRawData()
    {
        string testData = "this is test data: ".RandomLetters(10);

        When.A<FsRawStorage>("saves and retrieves raw data",
            () => new FsRawStorage($"{nameof(OpaqueFsRawStorageShould)}_{nameof(SaveAndRetrieveRawData)}"),
            (storage) =>
            {
                IRawData data = new RawData(testData);
                IStorageSlot slot = storage.Save(data);

                byte[] value = slot.GetData()!.Value;
                string retrievedFromSlot = Encoding.UTF8.GetString(value);

                IRawData rawFromStorage = storage.LoadHashHexString(data.HashHexString);
                string retrievedFromStorage = Encoding.UTF8.GetString(rawFromStorage.Value);

                return new object[] { retrievedFromSlot, retrievedFromStorage };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            string retrievedFromSlot = (string)results[0];
            string retrievedFromStorage = (string)results[1];
            because.ItsTrue("retrieved from slot is not null", retrievedFromSlot != null);
            because.ItsTrue("retrieved from slot equals original", testData.Equals(retrievedFromSlot));
            because.ItsTrue("retrieved from storage is not null", retrievedFromStorage != null);
            because.ItsTrue("retrieved from storage equals original", testData.Equals(retrievedFromStorage));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
