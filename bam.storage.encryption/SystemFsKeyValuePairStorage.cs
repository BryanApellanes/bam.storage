using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class SystemFsKeyValuePairStorage : OpaqueFsKeyValuePairStorage
{
    public SystemFsKeyValuePairStorage() : base(new FsSlottedStorage(BamProfile.VaultsDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}