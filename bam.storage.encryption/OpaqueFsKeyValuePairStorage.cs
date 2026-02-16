
using Bam.Encryption;
using System.Text;

namespace Bam.Storage.Encryption;

/// <summary>
/// An encrypted key-value pair storage that obfuscates keys using HMAC and encrypts values using AES,
/// providing opaque on-disk storage where neither keys nor values are readable without the correct cryptographic keys.
/// </summary>
public class OpaqueFsKeyValuePairStorage : IKeyValuePairStorage
{
    /// <summary>
    /// Creates an instance of the OpaqueKeyValuePairStorage.
    /// </summary>
    /// <param name="slottedStorage">The filesystem storage.</param>
    /// <param name="aesKeySource">The provider for the AES key used to encrypt values.</param>
    /// <param name="hmacKeyProvider">The provider for hmac keys used to obfuscate keys.</param>
    /// <remarks>Note that "Key" in this context is not a cryptographic key but the left value of a dictionary access mechanism used to access an associated value.</remarks>
    public OpaqueFsKeyValuePairStorage(FsSlottedStorage slottedStorage, IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider)
    {
        this.PairStorage = new FsKeyValuePairStorage(slottedStorage);
        this.AesKeySource =  aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }

    /// <summary>
    /// Gets the underlying unencrypted file-system key-value pair storage.
    /// </summary>
    protected FsKeyValuePairStorage PairStorage { get; }

    /// <summary>
    /// Gets the AES key source used for encrypting and decrypting values.
    /// </summary>
    protected IAesKeySource AesKeySource { get; }

    /// <summary>
    /// Gets or sets the HMAC key provider used for obfuscating keys.
    /// </summary>
    protected IHmacKeyProvider HmacKeyProvider
    {
        get;
        set;
    }

    /// <summary>
    /// Transforms a key-value pair by obfuscating the key with HMAC and encrypting the value with AES.
    /// </summary>
    /// <param name="keyValuePair">The original key-value pair.</param>
    /// <returns>A new key-value pair with the transformed key and encrypted value.</returns>
    protected virtual IKeyValuePair Transform(IKeyValuePair keyValuePair)
    {
        return new KeyValuePair()
        {
            Key = TransformKey(keyValuePair.Key),
            Value = EncryptValue(keyValuePair.Value),
        };
    }

    /// <summary>
    /// Saves a key-value pair by obfuscating the key and encrypting the byte array value.
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The byte array value to encrypt and store.</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    public IKeyValuePairSaveResult Save(string key, byte[] value)
    {
        return Save(new KeyValuePair(key, value));
    }

    /// <summary>
    /// Saves a key-value pair by obfuscating the key and encrypting the string value (encoded as UTF-8).
    /// </summary>
    /// <param name="key">The key to store the value under.</param>
    /// <param name="value">The string value to encrypt and store.</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    public IKeyValuePairSaveResult Save(string key, string value)
    {
        return Save(new KeyValuePair(key, value));
    }

    /// <summary>
    /// Saves the specified key-value pair by transforming the key (HMAC) and encrypting the value (AES).
    /// </summary>
    /// <param name="keyValuePair">The key-value pair to save.</param>
    /// <returns>A result indicating whether the save was successful.</returns>
    public IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair)
    {
        return PairStorage.Save(Transform(keyValuePair));
    }

    /// <summary>
    /// Retrieves and decrypts the value associated with the specified key. The key is transformed
    /// via HMAC to locate the stored data, then the value is decrypted using AES.
    /// </summary>
    /// <param name="key">The original (un-transformed) key to look up.</param>
    /// <returns>A key-value pair with the original key and decrypted value.</returns>
    public virtual IKeyValuePair Get(string key)
    {
        string transformedKey = TransformKey(key);
        IKeyValuePair keyValuePair = PairStorage.Get(transformedKey);
        AesKey aesKey = AesKeySource.GetAesKey();
        return new KeyValuePair()
        {
            Key = key,
            Value = aesKey.DecryptBytes(keyValuePair.Value)
        };
    }

    /// <summary>
    /// Transforms the key into an opaque identifier using double HMAC-SHA256, making it unrecoverable from the stored form.
    /// </summary>
    /// <param name="key">The original key to transform.</param>
    /// <returns>A Base64-encoded opaque key identifier.</returns>
    protected virtual string TransformKey(string key)
    {
        byte[] hmacKey = HmacKeyProvider.GetNamedHmacKey(nameof(OpaqueFsKeyValuePairStorage));
        return key.DoubleHmacSha256(hmacKey).ToBase64();
    }

    /// <summary>
    /// Encrypts a byte array value using AES encryption.
    /// </summary>
    /// <param name="value">The plaintext byte array to encrypt.</param>
    /// <returns>The AES-encrypted byte array.</returns>
    protected virtual byte[] EncryptValue(byte[] value)
    {
        AesKey aesKey = AesKeySource.GetAesKey();
        return aesKey.EncryptBytes(value);
    }
}