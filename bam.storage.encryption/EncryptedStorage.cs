using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class EncryptedStorage : IEncryptedStorage
{
    public EncryptedStorage(IStorage storage, IEncryptor encryptor)
    {
        this.Storage = storage;
        this.Encryptor = encryptor;
    }
    
    protected IStorage Storage { get; set; }
    public IEncryptor Encryptor { get; set; }
    
    public IStorageSlot Save(IRawData rawData)
    {
        byte[] encrypted = Encryptor.Encrypt(rawData.Value);
        return this.Storage.Save(encrypted);
    }

    public IRawData LoadHashId(ulong hashId)
    {
        throw new NotImplementedException();
    }

    public IRawData LoadHashHexString(string hashHexString)
    {
        //hashHexString.FromHexString();
        throw new NotImplementedException();
    }
}