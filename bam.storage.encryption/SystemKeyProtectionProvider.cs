using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// A protection provider that uses the system-level AES key (<see cref="AesKey.SystemKey"/>) for protecting sensitive data on disk.
/// </summary>
public class SystemKeyProtectionProvider : IProtectionProvider
{
    /// <summary>
    /// Gets the system-level AES key used for data protection.
    /// </summary>
    /// <returns>The system AES key.</returns>
    public AesKey GetProtectionKey()
    {
        return AesKey.SystemKey;
    }
}