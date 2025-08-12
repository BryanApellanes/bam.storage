using System.Text;
using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class OpaqueFsRawStorage : FsRawStorage
{
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider) : base()
    {
        this.AesKeySource = aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }
    
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider, IStorageHolder rootHolder) : base(rootHolder)
    {
        this.AesKeySource = aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }
    
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider, string rootPath) : base(rootPath)
    {
        this.AesKeySource = aesKeySource;
        this.HmacKeyProvider = hmacKeyProvider;
    }

    protected IAesKeySource AesKeySource { get; set; }
    protected IHmacKeyProvider HmacKeyProvider { get; set; }
    
    public IRawData LoadHashHexString(string hashHexString)
    {
        string hmacPath = TransformHashHexString(hashHexString);
        IStorageSlot hmacSlot = FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hmacPath);
        IRawData encrypted = hmacSlot.GetData();
        AesKey aesKey = AesKeySource.GetAesKey();
        byte[] bytes = aesKey.DecryptBytes(encrypted.Value);
        return new RawData(bytes);
    }

    public override IStorageSlot Save(IRawData rawData)
    {
        string hmacPath = TransformHashHexString(rawData.HashHexString);
        IStorageSlot hmacSlot = FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hmacPath);
        AesKey aesKey = AesKeySource.GetAesKey();
        byte[] encryptedBytes = aesKey.EncryptBytes(rawData.Value);
        RawData encrypted = new RawData(encryptedBytes);
        hmacSlot.SetData(encrypted);
        return hmacSlot;
    }
    
    protected string TransformHashHexString(string hashHexString)
    {
        byte[] hmacKey = HmacKeyProvider.GetNamedHmacKey(nameof(OpaqueFsRawStorage));
        return hashHexString.DoubleHmacSha256(hmacKey.ToBase64()).ToHexString();
    }
}