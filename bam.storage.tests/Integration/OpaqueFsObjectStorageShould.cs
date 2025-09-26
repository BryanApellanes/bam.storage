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
    public async Task SaveAndRetrieve()
    {
        string testValue = 16.RandomLetters();
        ServiceRegistry registry = Configure(svcReg =>
        {
            svcReg.For<IAesKeySource>().UseSingleton(new AesKey());
            svcReg.For<IHmacKeyProvider>().Use<HmacKeyProvider>();
        });
        OpaqueFsSlottedStorage storage = registry.Get<OpaqueFsSlottedStorage>();
        RawData data = new RawData(testValue);
        IStorageSlot slot = storage.Save(data);

        IRawData loaded = storage.LoadHashHexString(data.HashHexString);
        string value = Encoding.UTF8.GetString(loaded.Value);
        
        value.ShouldEqual(testValue);
        Message.PrintLine(testValue);
    }
}