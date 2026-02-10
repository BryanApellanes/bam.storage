using System.Text;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Integration;

[UnitTestMenu("SystemKeyValuePairStorageShould")]
public class SystemKeyValuePairStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public void SaveAndRetrieve()
    {
        When.A<SystemFsKeyValuePairStorage>("saves and retrieves a system key-value pair",
            (storage) =>
            {
                storage.Save(new Bam.Storage.KeyValuePair("key1", Encoding.UTF8.GetBytes("value1")));
                IKeyValuePair kv = storage.Get("key1");
                string value = Encoding.UTF8.GetString(kv.Value);
                return value;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("retrieved value equals expected", "value1".Equals((string)because.Result));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
