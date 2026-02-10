using System.Text;
using Bam.Console;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Integration;

[UnitTestMenu("OpaqueFsObjectStorageShould")]
public class OpaqueFsObjectStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public void SaveAndRetrieve()
    {
        string testValue = 16.RandomLetters();

        When.A<OpaqueFsSlottedStorage>("saves and retrieves opaque data",
            () =>
            {
                ServiceRegistry registry = Configure(svcReg =>
                {
                    svcReg.For<IAesKeySource>().UseSingleton(new AesKey());
                    svcReg.For<IHmacKeyProvider>().Use<HmacKeyProvider>();
                });
                return registry.Get<OpaqueFsSlottedStorage>();
            },
            (storage) =>
            {
                RawData data = new RawData(testValue);
                IStorageSlot slot = storage.Save(data);
                IRawData loaded = storage.LoadHashHexString(data.HashHexString);
                string value = Encoding.UTF8.GetString(loaded.Value);
                return value;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("retrieved value equals original", testValue.Equals((string)because.Result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
