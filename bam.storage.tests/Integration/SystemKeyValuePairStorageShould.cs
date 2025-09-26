using System.Text;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Integration;


[UnitTestMenu("SystemKeyValuePairStorageShould")]
public class SystemKeyValuePairStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public async Task SaveAndRetrieve()
    {
        SystemFsKeyValuePairStorage storage = new SystemFsKeyValuePairStorage();
        storage.Save(new Bam.Storage.KeyValuePair("key1", Encoding.UTF8.GetBytes("value1")));

        IKeyValuePair kv = storage.Get("key1");
        
        string value = Encoding.UTF8.GetString(kv.Value);
        
        value.ShouldBeEqualTo("value1");
    }
    
}