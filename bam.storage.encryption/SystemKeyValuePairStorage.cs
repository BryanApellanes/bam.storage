using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class SystemKeyValuePairStorage : OpaqueKeyValuePairStorage
{
    public SystemKeyValuePairStorage() : base(new FsObjectStorage(BamProfile.VaultsDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}