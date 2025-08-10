using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class SystemKeyProtectionProvider : IProtectionProvider
{
    public AesKey GetProtectionKey()
    {
        return AesKey.SystemKey;
    }
}