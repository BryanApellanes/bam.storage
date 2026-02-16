using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// Provides encryption, decryption, and hash transformation operations that make stored data opaque.
/// Uses AES for data encryption and double HMAC-SHA256 for obfuscating storage paths.
/// </summary>
public class OpaquenessProvider
{
    /// <summary>
    /// Initializes a new instance of <see cref="OpaquenessProvider"/> with the specified AES key source and HMAC key provider.
    /// </summary>
    /// <param name="aesKeySource">The source for AES keys used in data encryption and decryption.</param>
    /// <param name="hmacKeyProvider">The provider for HMAC keys used to transform hash hex strings.</param>
    public OpaquenessProvider(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider)
    {
        this.AesKeySource = aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }

    /// <summary>
    /// Gets or sets the AES key source used for encrypting and decrypting data.
    /// </summary>
    public IAesKeySource AesKeySource { get; set; }

    /// <summary>
    /// Gets or sets the HMAC key provider used for transforming hash hex strings into opaque identifiers.
    /// </summary>
    public IHmacKeyProvider HmacKeyProvider { get; set; }

    /// <summary>
    /// Transforms a hash hex string into an opaque identifier using double HMAC-SHA256, making the original
    /// hash unrecoverable from the storage path.
    /// </summary>
    /// <param name="hashHexString">The original hex-encoded hash string.</param>
    /// <returns>A hex-encoded opaque identifier derived from the hash.</returns>
    public string TransformHashHexString(string hashHexString)
    {
        byte[] hmacKey = HmacKeyProvider.GetNamedHmacKey(nameof(OpaqueFsRawStorage));
        return hashHexString.DoubleHmacSha256(hmacKey.ToBase64()).ToHexString();
    }

    /// <summary>
    /// Encrypts the specified raw data using AES encryption.
    /// </summary>
    /// <param name="rawData">The raw data to encrypt.</param>
    /// <returns>A new <see cref="IRawData"/> containing the encrypted bytes.</returns>
    public IRawData Encrypt(IRawData rawData)
    {
        AesKey aesKey = AesKeySource.GetAesKey();
        byte[] encryptedBytes = aesKey.EncryptBytes(rawData.Value);
        return new RawData(encryptedBytes);
    }

    /// <summary>
    /// Decrypts the specified raw data using AES decryption.
    /// </summary>
    /// <param name="rawData">The raw data containing encrypted bytes.</param>
    /// <returns>A new <see cref="IRawData"/> containing the decrypted bytes.</returns>
    public IRawData Decrypt(IRawData rawData)
    {
        AesKey aesKey = AesKeySource.GetAesKey();
        byte[] decryptedBytes = aesKey.DecryptBytes(rawData.Value);
        return new RawData(decryptedBytes);
    }
}