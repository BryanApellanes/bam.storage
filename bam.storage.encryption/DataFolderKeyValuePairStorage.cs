using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class DataFolderKeyValuePairStorage : OpaqueKeyValuePairStorage
{
    public DataFolderKeyValuePairStorage() :
        base(new FsObjectStorage(BamProfile.DataDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}