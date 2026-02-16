using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// Provides an AES key used to protect sensitive data (such as private keys) when stored on disk.
/// </summary>
public interface IProtectionProvider
{
    /// <summary>
    /// Gets the AES key used for encrypting and decrypting protected data.
    /// </summary>
    /// <returns>The protection AES key.</returns>
    AesKey GetProtectionKey();
}