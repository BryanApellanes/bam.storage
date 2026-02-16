using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// An opaque key-value pair storage that uses the user profile's data.sys directory as its root,
/// with the system key set for AES encryption and HMAC key obfuscation.
/// </summary>
public class DataFolderOpaqueFsKeyValuePairStorage : OpaqueFsKeyValuePairStorage
{
    /// <summary>
    /// Initializes a new instance of <see cref="DataFolderOpaqueFsKeyValuePairStorage"/> using the
    /// profile data.sys directory, system key set, and default HMAC key provider.
    /// </summary>
    public DataFolderOpaqueFsKeyValuePairStorage() :
        base(new FsSlottedStorage(BamProfile.DataDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}