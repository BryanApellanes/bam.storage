using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class OpaquenessProvider
{
    public OpaquenessProvider(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider)
    {
        this.AesKeySource = aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }
    
    public IAesKeySource AesKeySource { get; set; }
    public IHmacKeyProvider HmacKeyProvider { get; set; }

    public string TransformHashHexString(string hashHexString)
    {
        byte[] hmacKey = HmacKeyProvider.GetNamedHmacKey(nameof(OpaqueFsRawStorage));
        return hashHexString.DoubleHmacSha256(hmacKey.ToBase64()).ToHexString();
    }

    public IRawData Encrypt(IRawData rawData)
    {
        AesKey aesKey = AesKeySource.GetAesKey();
        byte[] encryptedBytes = aesKey.EncryptBytes(rawData.Value);
        return new RawData(encryptedBytes);
    }

    public IRawData Decrypt(IRawData rawData)
    {
        AesKey aesKey = AesKeySource.GetAesKey();
        byte[] decryptedBytes = aesKey.DecryptBytes(rawData.Value);
        return new RawData(decryptedBytes);
    }
}