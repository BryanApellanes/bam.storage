using Bam.Encryption;
using System.Text;

namespace Bam.Storage.Encryption;

/// <summary>
/// Provides basic access to the system's ECC and RSA key pairs stored in the vault.sys files.
/// System level protection of the files should be limited to the system user account only.
/// The IProtectionProvider is used to protect the private keys from casual access when stored on disk.
/// </summary>
public class SystemKeySet : IAesKeySource, IRsaKeySource
{
    private const string privateEccKeyFile = "eccpr.sys";
    private const string publicEccKeyFile = "eccpu.sys";
    private const string privateRsaKeyFile = "rsapr.sys";
    private const string publicRsaKeyFile = "rsapu.sys";
    
    public SystemKeySet(IProtectionProvider protectionProvider, Encoding? encoding = null)
    {
        this.ProtectionProvider = protectionProvider;
        if (TryReadPrivateEccKeyCipher(out string eccPrivateKeyPemCipher))
        {
            AesKey key = protectionProvider.GetProtectionKey();
            byte[] eccPrivateKeyPem = (encoding ?? Encoding.UTF8).GetBytes(key.Decrypt(eccPrivateKeyPemCipher));
            EccPrivateKeyPem = eccPrivateKeyPem;
        }

        if (BamProfile.TryReadVaultDotSysFileString(publicEccKeyFile, out string eccPublicKeyPem))
        {
            EccPublicKeyPem = eccPublicKeyPem;
        }
        
        if (TryReadPrivateRsaKeyCipher(out string rsaPrivateKeyPemCipher))
        {
            AesKey key = protectionProvider.GetProtectionKey();
            byte[] rsaPrivateKeyPem = (encoding ?? Encoding.UTF8).GetBytes(key.Decrypt(rsaPrivateKeyPemCipher));
            RsaPrivateKeyPem = rsaPrivateKeyPem;
        }

        if (BamProfile.TryReadVaultDotSysFileString(publicRsaKeyFile, out string rsapublicKeyPem))
        {
            RsaPublicKeyPem = rsapublicKeyPem;
        }
    }

    protected virtual bool TryReadPrivateEccKeyCipher(out string eccPrivateKeyPemCipher)
    {
        return BamProfile.TryReadVaultDotSysFileString(privateEccKeyFile, out eccPrivateKeyPemCipher);
    }

    protected virtual bool TryReadPrivateRsaKeyCipher(out string rsaPrivateKeyPemCipher)
    {
        return BamProfile.TryReadVaultDotSysFileString(privateRsaKeyFile, out rsaPrivateKeyPemCipher);
    }
    
    protected IProtectionProvider ProtectionProvider { get; set; }

    private static Lazy<SystemKeySet> _current = new Lazy<SystemKeySet>(new SystemKeySet(new SystemKeyProtectionProvider()));

    public static SystemKeySet Current
    {
        get => _current.Value;
        set => _current = new Lazy<SystemKeySet>(value);
    }
    
    public byte[] EccPrivateKeyPem { get; set; }
    public string EccPublicKeyPem { get; set; }
    
    public byte[] RsaPrivateKeyPem { get; set; }
    public string RsaPublicKeyPem { get; set; }

    private EccKeyPair _eccKeyPair;
    
    public EccKeyPair GetEccKeyPair()
    {
        if (_eccKeyPair != null)
        {
            return _eccKeyPair;
        }
        EccKeyPair eccKeyPair;
        if (EccPrivateKeyPem != null && EccPrivateKeyPem.Length > 0)
        {
            eccKeyPair = new EccKeyPair(new EccPublicPrivateKeyPair(EccPrivateKeyPem));
            EccPublicKeyPem = eccKeyPair.PublicKey.Value.ToPem();
        }
        else
        {
            eccKeyPair = new EccKeyPair();
            EccPrivateKeyPem = eccKeyPair.Pem;
            EccPublicKeyPem = eccKeyPair.PublicPem;
        }
        _eccKeyPair = eccKeyPair;
        WriteEccPrivateKey();
        BamProfile.WriteVaultDotSysFile(publicEccKeyFile, EccPublicKeyPem);
        return eccKeyPair;
    }
    
    public AesKey GetAesKey()
    {
        return GetEccKeyPair().GetSelfAesKey();
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
        if (EccPrivateKeyPem != null && EccPrivateKeyPem.Length > 0)
        {
            rsaKeyPair = new RsaKeyPair(new RsaPublicPrivateKeyPair(RsaPrivateKeyPem));
            RsaPublicKeyPem = rsaKeyPair.PublicKey.Value.ToPem();
        }
        else
        {
            rsaKeyPair = new RsaKeyPair();
            RsaPrivateKeyPem = rsaKeyPair.Pem;
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
        byte[] cipher = key.EncryptBytes(EccPrivateKeyPem);
        BamProfile.WriteVaultDotSysFile(privateEccKeyFile, cipher.ToBase64());
    }

    protected virtual void WriteRsaPrivateKey()
    {
        AesKey key = ProtectionProvider.GetProtectionKey();
        byte[] cipher = key.EncryptBytes(RsaPrivateKeyPem);
        BamProfile.WriteVaultDotSysFile(privateRsaKeyFile, cipher.ToBase64());
    }
}