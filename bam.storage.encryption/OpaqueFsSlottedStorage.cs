using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// An opaque (encrypted) file-system slotted storage that encrypts data before saving and decrypts on load.
/// When saving to a hash-addressed slot, the storage path is also obfuscated via HMAC transformation.
/// </summary>
public class OpaqueFsSlottedStorage : FsSlottedStorage
{
    /// <summary>
    /// Initializes a new instance of <see cref="OpaqueFsSlottedStorage"/> using the default storage directory.
    /// </summary>
    /// <param name="aesKeySource">The source for AES keys used in data encryption and decryption.</param>
    /// <param name="hmacKeyProvider">The provider for HMAC keys used to obfuscate hash-based storage paths.</param>
    public OpaqueFsSlottedStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider) : base()
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }

    /// <summary>
    /// Gets or sets the provider that handles encryption, decryption, and hash path transformation.
    /// </summary>
    protected OpaquenessProvider OpaquenessProvider { get; set; }

    /// <summary>
    /// Encrypts and saves raw data to the specified slot. If the slot name matches the data's hash hex string,
    /// it is replaced with an HMAC-transformed opaque slot.
    /// </summary>
    /// <param name="slot">The target storage slot.</param>
    /// <param name="rawData">The raw data to encrypt and save.</param>
    /// <returns>The storage slot where the encrypted data was saved.</returns>
    public override IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        string slotKey = slot.Name.Replace("\\", "").Replace("/", "");
        if (slotKey == rawData.HashHexString)
        {
            slot = GetHashHexStringStorageSlot(rawData.HashHexString);
        }
        IRawData encrypted = OpaquenessProvider.Encrypt(rawData);
        slot.SetData(encrypted);
        return slot;
    }

    /// <summary>
    /// Loads encrypted data from the base storage using the hash hex string and decrypts it.
    /// </summary>
    /// <param name="hashHexString">The hex-encoded hash string identifying the data.</param>
    /// <returns>The decrypted raw data.</returns>
    public override IRawData LoadHashHexString(string hashHexString)
    {
        IRawData encrypted = base.LoadHashHexString(hashHexString);
        return OpaquenessProvider.Decrypt(encrypted);
    }

    /// <summary>
    /// Gets a storage slot with an HMAC-transformed segmented path derived from the hash hex string,
    /// making the on-disk path opaque.
    /// </summary>
    /// <param name="hashHexString">The original hex-encoded hash string.</param>
    /// <returns>An opaque file-system storage slot at the transformed path.</returns>
    public override IStorageSlot GetHashHexStringStorageSlot(string hashHexString)
    {
        Args.ThrowIfNullOrEmpty(hashHexString, nameof(hashHexString));
        string doubleHmac = OpaquenessProvider.TransformHashHexString(hashHexString);
        
        List<string> parts = new List<string>();
        parts.AddRange(doubleHmac.Split(2));
        parts.Add("dat");
        return new OpaqueFsStorageSlot(OpaquenessProvider, RootHolder, Path.Combine(parts.ToArray()));
    }
}