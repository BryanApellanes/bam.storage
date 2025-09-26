using System.Text;
using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class OpaqueFsRawStorage : FsRawStorage
{
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider) : base()
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }
    
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider, IStorageHolder rootHolder) : base(rootHolder)
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }
    
    public OpaqueFsRawStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider, string rootPath) : base(rootPath)
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }

    protected OpaquenessProvider OpaquenessProvider { get; set; }
    
    public IRawData LoadHashHexString(string hashHexString)
    {
        string hmacPath = OpaquenessProvider.TransformHashHexString(hashHexString);//TransformHashHexString(hashHexString);
        IStorageSlot hmacSlot = FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, hmacPath);
        IRawData encrypted = hmacSlot.GetData();
        return OpaquenessProvider.Decrypt(encrypted);
    }

    public override IStorageSlot Save(IRawData rawData)
    {
        string doubleHmac = OpaquenessProvider.TransformHashHexString(rawData.HashHexString);//TransformHashHexString(rawData.HashHexString);
        IStorageSlot hmacSlot = FsStorageSlot.GetSegmentedPathStorageSlot(RootHolder, doubleHmac);
        IRawData encrypted = OpaquenessProvider.Encrypt(rawData);
        hmacSlot.SetData(encrypted);
        return hmacSlot;
    }
}