using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class DataFolderKeyValuePairStorage : OpaqueKeyValuePairStorage
{
    public DataFolderKeyValuePairStorage() :
        base(new FsStorage(BamProfile.DataDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}