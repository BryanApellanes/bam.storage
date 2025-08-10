
using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class OpaqueKeyValuePairStorage : IKeyValuePairStorage
{
    /// <summary>
    /// Creates an instance of the OpaqueKeyValuePairStoragee.
    /// </summary>
    /// <param name="storage">The filesystem storage.</param>
    /// <param name="aesKeySource">The provider for the AES key used to encrypt values.</param>
    /// <param name="hmacKeyProvider">The provider for hmac keys used to obfuscate keys.</param>
    /// <remarks>Note that "Key" in this context is not a cryptographic key but the left value of a dictionary access mechanism used to access an associated value.</remarks>
    public OpaqueKeyValuePairStorage(FsStorage storage, IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider)
    {
        this.PairStorage = new FsKeyValuePairStorage(storage);
        this.AesKeySource =  aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }

    protected FsKeyValuePairStorage PairStorage { get; }
    protected IAesKeySource AesKeySource { get; }
    protected IHmacKeyProvider HmacKeyProvider
    {
        get;
        set;
    }
    
    protected virtual IKeyValuePair Transform(IKeyValuePair keyValuePair)
    {
        return new KeyValuePair()
        {
            Key = TransformKey(keyValuePair.Key),
            Value = TransformValue(keyValuePair.Value),
        };
    }
    
    public IKeyValuePairSaveResult Save(IKeyValuePair keyValuePair)
    {
        return PairStorage.Save(Transform(keyValuePair));
    }

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

    protected virtual string TransformKey(string key)
    {
        byte[] hmacKey = HmacKeyProvider.GetNamedHmacKey(nameof(OpaqueKeyValuePairStorage));
        return key.DoubleHmacSha256(hmacKey.ToBase64()).ToBase64();
    }

    protected virtual byte[] TransformValue(byte[] value)
    {
        AesKey aesKey = AesKeySource.GetAesKey();
        return aesKey.EncryptBytes(value);
    }
}