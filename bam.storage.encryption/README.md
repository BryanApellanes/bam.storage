# bam.storage.encryption

An encrypted storage layer that wraps bam.storage with AES encryption and HMAC-based key obfuscation, providing "opaque" filesystem storage where both data content and storage paths are cryptographically protected.

## Overview

bam.storage.encryption builds on top of bam.storage by introducing the concept of "opaque" storage. When data is saved, two transformations occur: the storage path (derived from the data's hash) is replaced with an HMAC-derived path (using double HMAC-SHA256), and the data content itself is encrypted with AES. This ensures that neither the filenames nor the file contents reveal anything about the original data.

The project provides opaque variants of the three main storage abstractions: `OpaqueFsRawStorage` for content-addressable encrypted storage, `OpaqueFsSlottedStorage` for slotted encrypted storage, and `OpaqueFsKeyValuePairStorage` for encrypted key-value pair persistence. The `OpaquenessProvider` class centralizes the path-obfuscation and encrypt/decrypt logic, taking an `IAesKeySource` for content encryption and an `IHmacKeyProvider` for path hashing.

The project also includes `SystemKeySet`, a singleton that manages the system's ECC and RSA key pairs stored in the BAM profile vault directory. `SystemKeySet` implements `IAesKeySource` (deriving AES keys from ECC shared secrets) and `IRsaKeySource`, and uses an `IProtectionProvider` to encrypt private keys at rest. Pre-configured convenience classes `DataFolderOpaqueFsKeyValuePairStorage` and `SystemFsKeyValuePairStorage` provide zero-configuration encrypted key-value stores rooted in standard BAM profile directories.

## Key Classes

| Class | Description |
|-------|-------------|
| `OpaquenessProvider` | Core class that performs path obfuscation (double HMAC-SHA256 on hash hex strings) and AES encrypt/decrypt on `IRawData`. |
| `OpaqueFsRawStorage` | Extends `FsRawStorage` to encrypt data and obfuscate storage paths before writing to disk. |
| `OpaqueFsSlottedStorage` | Extends `FsSlottedStorage` with encrypted save/load and HMAC-derived slot paths. |
| `OpaqueFsStorageSlot` | Extends `FsStorageSlot` to carry an `OpaquenessProvider` reference for downstream use. |
| `OpaqueFsKeyValuePairStorage` | Encrypted key-value pair storage: keys are HMAC-transformed, values are AES-encrypted. Wraps `FsKeyValuePairStorage`. |
| `DataFolderOpaqueFsKeyValuePairStorage` | Pre-configured `OpaqueFsKeyValuePairStorage` rooted in `BamProfile.DataDotSys` using system keys. |
| `SystemFsKeyValuePairStorage` | Pre-configured `OpaqueFsKeyValuePairStorage` rooted in `BamProfile.VaultsDotSys` using system keys. |
| `SystemKeySet` | Singleton managing the system's ECC and RSA key pairs. Implements `IAesKeySource` and `IRsaKeySource`. Stores encrypted private keys and plaintext public keys in the vault.sys directory. |
| `SystemKeyProtectionProvider` | Default `IProtectionProvider` that returns `AesKey.SystemKey` for protecting private keys at rest. |
| `IProtectionProvider` | Interface for providing an `AesKey` used to protect private key material. |
| `RsaPrivateKeyOpaqueStorage` | Stores and loads RSA private keys via opaque key-value storage. Supports named keys (`SaveNamedKey`/`GetNamedKey`) and scoped key usage via `UseNamedKey`, which decrypts the key in a `RsaPrivateKeyUsageContext` and disposes it after use. |

## Dependencies

**Project References:**
- `bam.storage` -- base storage abstractions (`FsRawStorage`, `FsSlottedStorage`, `FsKeyValuePairStorage`, `RawData`, etc.)
- `bam.encryption` -- cryptographic primitives (`AesKey`, `IAesKeySource`, `IHmacKeyProvider`, `HmacKeyProvider`, `EccKeyPair`, `RsaKeyPair`, etc.)

**Package References:**
- None (transitively uses BouncyCastle.Cryptography via bam.encryption)

## Usage Examples

### Opaque raw storage (content-addressable, encrypted)

```csharp
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;

// Create an opaque raw storage with a fresh AES key and HMAC provider
var storage = new OpaqueFsRawStorage(
    new AesKey(),
    new HmacKeyProvider(),
    "/path/to/encrypted-storage"
);

// Save data -- path is HMAC-obfuscated, content is AES-encrypted
IRawData data = new RawData("sensitive data");
IStorageSlot slot = storage.Save(data);

// Load by original hash -- internally translates to HMAC path and decrypts
IRawData loaded = storage.LoadHashHexString(data.HashHexString);
string text = System.Text.Encoding.UTF8.GetString(loaded.Value); // "sensitive data"
```

### Opaque slotted storage with dependency injection

```csharp
using Bam.Encryption;
using Bam.Storage.Encryption;

// Register dependencies
serviceRegistry.For<IAesKeySource>().UseSingleton(new AesKey());
serviceRegistry.For<IHmacKeyProvider>().Use<HmacKeyProvider>();

// Resolve and use
var storage = serviceRegistry.Get<OpaqueFsSlottedStorage>();
RawData data = new RawData("confidential payload");
IStorageSlot slot = storage.Save(data);
IRawData loaded = storage.LoadHashHexString(data.HashHexString);
```

### Pre-configured encrypted key-value storage

```csharp
using Bam.Storage;
using Bam.Storage.Encryption;

// Uses system ECC keys and HMAC keys from the BAM profile vault
var kvStore = new DataFolderOpaqueFsKeyValuePairStorage();

// Save an encrypted key-value pair
kvStore.Save("secretKey", "secretValue");

// Retrieve it -- key is HMAC-transformed for lookup, value is AES-decrypted
IKeyValuePair pair = kvStore.Get("secretKey");
string value = System.Text.Encoding.UTF8.GetString(pair.Value); // "secretValue"
```

### Using SystemKeySet for AES and RSA keys

```csharp
using Bam.Encryption;
using Bam.Storage.Encryption;

// Get the system AES key (derived from ECC self-shared secret)
AesKey aesKey = SystemKeySet.Current.GetAesKey();
string cipher = aesKey.Encrypt("plaintext");
string decrypted = aesKey.Decrypt(cipher);

// Get the system RSA public key
RsaPublicKey publicKey = SystemKeySet.Current.GetRsaPublicKey();
```

### Storing and using RSA private keys

```csharp
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;

// Create opaque storage for RSA private keys
var opaqueKvStorage = new OpaqueFsKeyValuePairStorage(
    new FsSlottedStorage("/path/to/key-storage"),
    new AesKey(),
    new HmacKeyProvider()
);
var keyStorage = new RsaPrivateKeyOpaqueStorage(opaqueKvStorage);

// Generate and save a named key
RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
keyStorage.SaveNamedKey("myServiceKey", keyPair.Pem);

// Later, use the key for signing or decryption without exposing the raw bytes
keyStorage.UseNamedKey("myServiceKey", (privateKey) =>
{
    string decrypted = new RsaPrivateKey(privateKey.Pem).Decrypt(cipherText);
});
```

## Known Gaps / Not Yet Implemented

- No known gaps at this time.
