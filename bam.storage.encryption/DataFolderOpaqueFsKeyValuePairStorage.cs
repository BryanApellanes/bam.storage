using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class DataFolderOpaqueFsKeyValuePairStorage : OpaqueFsKeyValuePairStorage
{
    public DataFolderOpaqueFsKeyValuePairStorage() :
        base(new FsSlottedStorage(BamProfile.DataDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}