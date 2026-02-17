using System.Text;
using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// An opaque (encrypted) file-system raw storage that encrypts data before saving and decrypts on load.
/// Storage paths are obfuscated using HMAC transformations so that neither the data content nor the
/// original hash-based path is visible on disk.
/// </summary>
public class OpaqueFsRawStorage : FsRawStorage
{
    /// <summary>
    /// Initializes a new instance of <see cref="OpaqueFsRawStorage"/> using the default working directory root.
    /// </summary>
    /// <param name="aesKeySource">The source for AES keys used in data encryption and decryption.</param>
    /// <param name="hmacKeyProvider">The provider for HMAC keys used to obfuscate storage paths.</param>
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider) : base()
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OpaqueFsRawStorage"/> using the specified storage holder as the root.
    /// </summary>
    /// <param name="aesKeySource">The source for AES keys used in data encryption and decryption.</param>
    /// <param name="hmacKeyProvider">The provider for HMAC keys used to obfuscate storage paths.</param>
    /// <param name="rootHolder">The root storage holder.</param>
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider, IStorageHolder rootHolder) : base(rootHolder)
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OpaqueFsRawStorage"/> using the specified directory path as the root.
    /// </summary>
    /// <param name="aesKeySource">The source for AES keys used in data encryption and decryption.</param>
    /// <param name="hmacKeyProvider">The provider for HMAC keys used to obfuscate storage paths.</param>
    /// <param name="rootPath">The root directory path.</param>
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider, string rootPath) : base(rootPath)
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }

    /// <summary>
    /// Gets or sets the provider that handles encryption, decryption, and hash path transformation.
    /// </summary>
    protected OpaquenessProvider OpaquenessProvider { get; set; }

    /// <summary>
    /// Loads and decrypts data identified by the specified hash hex string. The hash is first
    /// transformed via HMAC to locate the opaque storage path, then the encrypted data is decrypted.
    /// </summary>
    /// <param name="hashHexString">The original hex-encoded hash string identifying the data.</param>
    /// <returns>The decrypted raw data.</returns>
    public new IRawData LoadHashHexString(string hashHexString)
    {
        string hmacPath = OpaquenessProvider.TransformHashHexString(hashHexString);//TransformHashHexString(hashHexString);
        IStorageSlot hmacSlot = FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hmacPath);
        IRawData encrypted = hmacSlot.GetData()!;
        return OpaquenessProvider.Decrypt(encrypted)!;
    }

    /// <summary>
    /// Encrypts the raw data and saves it to an HMAC-transformed segmented path, making both
    /// the storage location and content opaque.
    /// </summary>
    /// <param name="rawData">The raw data to encrypt and save.</param>
    /// <returns>The storage slot where the encrypted data was saved.</returns>
    public override IStorageSlot Save(IRawData rawData)
    {
        string doubleHmac = OpaquenessProvider.TransformHashHexString(rawData.HashHexString);//TransformHashHexString(rawData.HashHexString);
        IStorageSlot hmacSlot = FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, doubleHmac);
        IRawData encrypted = OpaquenessProvider.Encrypt(rawData);
        hmacSlot.SetData(encrypted);
        return hmacSlot;
    }
}