using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// An opaque key-value pair storage that uses the user profile's vaults.sys directory as its root,
/// with the system key set for AES encryption and HMAC key obfuscation.
/// </summary>
public class SystemFsKeyValuePairStorage : OpaqueFsKeyValuePairStorage
{
    /// <summary>
    /// Initializes a new instance of <see cref="SystemFsKeyValuePairStorage"/> using the
    /// profile vaults.sys directory, system key set, and default HMAC key provider.
    /// </summary>
    public SystemFsKeyValuePairStorage() : base(new FsSlottedStorage(BamProfile.VaultsDotSys), SystemKeySet.Current, new HmacKeyProvider())
    {
    }
}