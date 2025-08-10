using Bam.Encryption;

namespace Bam.Storage.Encryption;

public class SystemKeySet : IAesKeySource, IRsaKeySource
{
    private const string privateEccKeyFile = "eccpr.sys";
    private const string publicEccKeyFile = "eccpu.sys";
    private const string privateRsaKeyFile = "rsapr.sys";
    private const string publicRsaKeyFile = "rsapu.sys";
    
    public SystemKeySet(IProtectionProvider protectionProvider)
    {
        this.ProtectionProvider = protectionProvider;
        if (BamProfile.TryReadVaultDotSysFile(privateEccKeyFile, out string eccPrivateKeyPemCipher))
        {
            AesKey key = protectionProvider.GetProtectionKey();
            string eccPrivateKeyPem = key.Decrypt(eccPrivateKeyPemCipher);
            EccPrivateKeyPem = eccPrivateKeyPem;
        }

        if (BamProfile.TryReadVaultDotSysFile(publicEccKeyFile, out string eccPublicKeyPem))
        {
            EccPublicKeyPem = eccPublicKeyPem;
        }
        
        if (BamProfile.TryReadVaultDotSysFile(privateRsaKeyFile, out string rsaPrivateKeyPemCipher))
        {
            AesKey key = protectionProvider.GetProtectionKey();
            string rsaPrivateKeyPem = key.Decrypt(rsaPrivateKeyPemCipher);
            RsaPrivateKeyPem = rsaPrivateKeyPem;
        }

        if (BamProfile.TryReadVaultDotSysFile(publicRsaKeyFile, out string rsaublicKeyPem))
        {
            RsaPublicKeyPem = eccPublicKeyPem;
        }
    }
    
    protected IProtectionProvider ProtectionProvider { get; set; }

    private static Lazy<SystemKeySet> _current = new Lazy<SystemKeySet>(new SystemKeySet(new SystemKeyProtectionProvider()));

    public static SystemKeySet Current
    {
        get => _current.Value;
        set => _current = new Lazy<SystemKeySet>(value);
    }
    
    public string EccPrivateKeyPem { get; set; }
    public string EccPublicKeyPem { get; set; }
    
    public string RsaPrivateKeyPem { get; set; }
    public string RsaPublicKeyPem { get; set; }

    private EccKeyPair _eccKeyPair;
    
    public EccKeyPair GetEccKeyPair()
    {
        if (_eccKeyPair != null)
        {
            return _eccKeyPair;
        }
        EccKeyPair eccKeyPair;
        if (!string.IsNullOrEmpty(EccPrivateKeyPem))
        {
            eccKeyPair = new EccKeyPair(new EccPublicPrivateKeyPair(EccPrivateKeyPem));
            EccPublicKeyPem = eccKeyPair.PublicKey.Value.ToPem();
        }
        else
        {
            eccKeyPair = new EccKeyPair();
            EccPrivateKeyPem = eccKeyPair.PrivatePem;
            EccPublicKeyPem = eccKeyPair.PublicPem;
        }
        _eccKeyPair = eccKeyPair;
        WriteEccPrivateKey();
        BamProfile.WriteVaultDotSysFile(publicEccKeyFile, EccPublicKeyPem);
        return eccKeyPair;
    }
    
    public AesKey GetAesKey()
    {
        return GetEccKeyPair().GetAesKey();
    }

    public AesKey GetSharedAesKey(string otherPublicPem)
    {
        return  GetEccKeyPair().GetSharedAesKey(otherPublicPem);
    }
    
    public RsaPublicKey GetRsaPublicKey()
    {
        return GetRsaKey().GetRsaPublicKey();
    }

    RsaKeyPair _rsaKeyPair;
    public RsaPublicPrivateKeyPair GetRsaKey()
    {
        if (_rsaKeyPair != null)
        {
            return _rsaKeyPair.Value;
        }
        RsaKeyPair rsaKeyPair;
        if (!string.IsNullOrEmpty(EccPrivateKeyPem))
        {
            rsaKeyPair = new RsaKeyPair(new RsaPublicPrivateKeyPair(RsaPrivateKeyPem));
            RsaPublicKeyPem = rsaKeyPair.PublicKey.Value.ToPem();
        }
        else
        {
            rsaKeyPair = new RsaKeyPair();
            RsaPrivateKeyPem = rsaKeyPair.PrivatePem;
            RsaPublicKeyPem = rsaKeyPair.PublicPem;
        }
        _rsaKeyPair = rsaKeyPair;
        WriteRsaPrivateKey();
        BamProfile.WriteVaultDotSysFile(publicRsaKeyFile, RsaPublicKeyPem);
        return _rsaKeyPair.Value;
    }

    protected virtual void WriteEccPrivateKey()
    {
        AesKey key = ProtectionProvider.GetProtectionKey();
        string cipher = key.Encrypt(EccPrivateKeyPem);
        BamProfile.WriteVaultDotSysFile(privateEccKeyFile, cipher);
    }

    protected virtual void WriteRsaPrivateKey()
    {
        AesKey key = ProtectionProvider.GetProtectionKey();
        string cipher = key.Encrypt(RsaPrivateKeyPem);
        BamProfile.WriteVaultDotSysFile(privateRsaKeyFile, cipher);
    }
}