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
    
    /// <summary>
    /// Initializes a new instance of <see cref="SystemKeySet"/> by reading and decrypting the ECC and RSA key pairs
    /// from the vault.sys directory using the specified protection provider.
    /// </summary>
    /// <param name="protectionProvider">The protection provider supplying the AES key used to decrypt stored private keys.</param>
    /// <param name="encoding">The text encoding to use when converting key PEM data, or null for UTF-8.</param>
    public SystemKeySet(IProtectionProvider protectionProvider, Encoding? encoding = null)
    {
        this.ProtectionProvider = protectionProvider;
        if (TryReadPrivateEccKeyCipher(out string eccPrivateKeyPemCipher))
        {
            AesKey key = protectionProvider.GetProtectionKey();
            byte[] eccPrivateKeyPem = (encoding ?? Encoding.UTF8).GetBytes(key.Decrypt(eccPrivateKeyPemCipher));
            EccPrivateKeyPem = eccPrivateKeyPem;
        }

        if (BamProfile.TryReadVaultDotSysFileString(publicEccKeyFile, out string? eccPublicKeyPem))
        {
            EccPublicKeyPem = eccPublicKeyPem!;
        }
        
        if (TryReadPrivateRsaKeyCipher(out string rsaPrivateKeyPemCipher))
        {
            AesKey key = protectionProvider.GetProtectionKey();
            byte[] rsaPrivateKeyPem = (encoding ?? Encoding.UTF8).GetBytes(key.Decrypt(rsaPrivateKeyPemCipher));
            RsaPrivateKeyPem = rsaPrivateKeyPem;
        }

        if (BamProfile.TryReadVaultDotSysFileString(publicRsaKeyFile, out string? rsapublicKeyPem))
        {
            RsaPublicKeyPem = rsapublicKeyPem!;
        }
    }

    protected virtual bool TryReadPrivateEccKeyCipher(out string eccPrivateKeyPemCipher)
    {
        bool result = BamProfile.TryReadVaultDotSysFileString(privateEccKeyFile, out string? value);
        eccPrivateKeyPemCipher = value!;
        return result;
    }

    protected virtual bool TryReadPrivateRsaKeyCipher(out string rsaPrivateKeyPemCipher)
    {
        bool result = BamProfile.TryReadVaultDotSysFileString(privateRsaKeyFile, out string? value);
        rsaPrivateKeyPemCipher = value!;
        return result;
    }
    
    protected IProtectionProvider ProtectionProvider { get; set; }

    private static Lazy<SystemKeySet> _current = new Lazy<SystemKeySet>(new SystemKeySet(new SystemKeyProtectionProvider()));

    /// <summary>
    /// Gets or sets the current global singleton <see cref="SystemKeySet"/> instance, lazily initialized
    /// using the <see cref="SystemKeyProtectionProvider"/>.
    /// </summary>
    public static SystemKeySet Current
    {
        get => _current.Value;
        set => _current = new Lazy<SystemKeySet>(value);
    }

    /// <summary>
    /// Gets or sets the ECC private key in PEM format as raw bytes.
    /// </summary>
    public byte[] EccPrivateKeyPem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ECC public key in PEM format as a string.
    /// </summary>
    public string EccPublicKeyPem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the RSA private key in PEM format as raw bytes.
    /// </summary>
    public byte[] RsaPrivateKeyPem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the RSA public key in PEM format as a string.
    /// </summary>
    public string RsaPublicKeyPem { get; set; } = null!;

    private EccKeyPair _eccKeyPair = null!;
    
    /// <summary>
    /// Gets the ECC key pair, loading it from the stored private key PEM if available, or generating a new one
    /// and persisting it to the vault.sys directory. The result is cached for subsequent calls.
    /// </summary>
    /// <returns>The ECC key pair.</returns>
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
    
    /// <summary>
    /// Gets an AES key derived from the ECC key pair's self-shared secret.
    /// </summary>
    /// <returns>An AES key derived from the ECC key pair.</returns>
    public AesKey GetAesKey()
    {
        return GetEccKeyPair().GetSelfAesKey();
    }

    /// <summary>
    /// Gets a shared AES key derived from this system's ECC key pair and another party's public key PEM.
    /// </summary>
    /// <param name="otherPublicPem">The other party's ECC public key in PEM format.</param>
    /// <returns>A shared AES key for secure communication with the other party.</returns>
    public AesKey GetSharedAesKey(string otherPublicPem)
    {
        return  GetEccKeyPair().GetSharedAesKey(otherPublicPem);
    }
    
    /// <summary>
    /// Gets the RSA public key extracted from the RSA key pair.
    /// </summary>
    /// <returns>The RSA public key.</returns>
    public RsaPublicKey GetRsaPublicKey()
    {
        return GetRsaKey().GetRsaPublicKey();
    }

    RsaKeyPair _rsaKeyPair = null!;
    /// <summary>
    /// Gets the RSA public-private key pair, loading it from the stored private key PEM if available,
    /// or generating a new one and persisting it to the vault.sys directory. The result is cached for subsequent calls.
    /// </summary>
    /// <returns>The RSA public-private key pair.</returns>
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