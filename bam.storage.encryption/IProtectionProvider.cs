using Bam.Encryption;

namespace Bam.Storage.Encryption;

public interface IProtectionProvider
{
    AesKey GetProtectionKey();
}