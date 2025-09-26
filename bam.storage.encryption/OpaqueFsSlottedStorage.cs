using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class OpaqueFsSlottedStorage : FsSlottedStorage
{
    public OpaqueFsSlottedStorage(IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider) : base()
    {
        this.OpaquenessProvider = new OpaquenessProvider(aesKeySource, hmacKeyProvider);
    }
    
    protected OpaquenessProvider OpaquenessProvider { get; set; }

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

    public override IRawData LoadHashHexString(string hashHexString)
    {
        IRawData encrypted = base.LoadHashHexString(hashHexString);
        return OpaquenessProvider.Decrypt(encrypted);
    }

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