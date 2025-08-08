using Bam.Encryption;

namespace Bam.Storage.Encryption;

public interface IEncryptedStorage : IRawStorage
{
    IEncryptor Encryptor { get; }
}