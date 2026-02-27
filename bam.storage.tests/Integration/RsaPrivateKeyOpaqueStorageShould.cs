using System.Text;
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Integration;

[UnitTestMenu("RsaPrivateKeyOpaqueStorageShould")]
public class RsaPrivateKeyOpaqueStorageShould : UnitTestMenuContainer
{
    private static RsaPrivateKeyOpaqueStorage CreateStorage(string testName)
    {
        OpaqueFsKeyValuePairStorage opaqueKvStorage = new OpaqueFsKeyValuePairStorage(
            new FsSlottedStorage($"RsaPrivateKeyOpaqueStorageShould_{testName}"),
            new AesKey(),
            new HmacKeyProvider()
        );
        return new RsaPrivateKeyOpaqueStorage(opaqueKvStorage);
    }

    [UnitTest]
    public void SaveAndGetNamedKeyRoundTrip()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("saves and retrieves a named key",
            () => CreateStorage(nameof(SaveAndGetNamedKeyRoundTrip)),
            (storage) =>
            {
                using RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
                byte[] originalBytes = (byte[])keyPair.Pem.Clone();

                IKeyValuePairSaveResult saveResult = storage.SaveNamedKey("myKey", originalBytes);
                byte[]? retrieved = storage.GetNamedKey("myKey");

                return new object[] { saveResult.Success, originalBytes, retrieved! };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool success = (bool)results[0];
            byte[] original = (byte[])results[1];
            byte[] retrieved = (byte[])results[2];
            because.ItsTrue("save succeeded", success);
            because.ItsTrue("retrieved is not null", retrieved != null);
            because.ItsTrue("retrieved bytes equal original", original.SequenceEqual(retrieved));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void WritePrivateKeyBytesUsesDefaultKeyName()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("writes private key bytes under default key name",
            () => CreateStorage(nameof(WritePrivateKeyBytesUsesDefaultKeyName)),
            (storage) =>
            {
                using RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
                byte[] originalBytes = (byte[])keyPair.Pem.Clone();

                bool writeResult = storage.WritePrivateKeyBytes(originalBytes);
                byte[]? retrieved = storage.GetNamedKey("DefaultRsaKey");

                return new object[] { writeResult, originalBytes, retrieved! };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool writeResult = (bool)results[0];
            byte[] original = (byte[])results[1];
            byte[] retrieved = (byte[])results[2];
            because.ItsTrue("write succeeded", writeResult);
            because.ItsTrue("retrieved is not null", retrieved != null);
            because.ItsTrue("retrieved bytes equal original", original.SequenceEqual(retrieved));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void WritePrivateKeyBytesFromKeyPair()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("writes private key bytes from key pair",
            () => CreateStorage(nameof(WritePrivateKeyBytesFromKeyPair)),
            (storage) =>
            {
                using RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
                byte[] expectedPem = (byte[])keyPair.Pem.Clone();

                bool writeResult = storage.WritePrivateKeyBytes(keyPair);
                byte[]? retrieved = storage.GetNamedKey("DefaultRsaKey");

                return new object[] { writeResult, expectedPem, retrieved! };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool writeResult = (bool)results[0];
            byte[] expectedPem = (byte[])results[1];
            byte[] retrieved = (byte[])results[2];
            because.ItsTrue("write succeeded", writeResult);
            because.ItsTrue("retrieved is not null", retrieved != null);
            because.ItsTrue("retrieved bytes equal key pair pem", expectedPem.SequenceEqual(retrieved));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReadPrivateKeyConstructsValidKeyPair()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("reads private key and constructs a valid key pair",
            () => CreateStorage(nameof(ReadPrivateKeyConstructsValidKeyPair)),
            (storage) =>
            {
                using RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
                byte[] pemBytes = originalKeyPair.Pem;

                using RsaPublicPrivateKeyPair reconstructed = storage.ReadPrivateKey(pemBytes);

                string plaintext = "test message for encryption";
                string encrypted = new RsaPublicKey(reconstructed.PublicKeyPem).Encrypt(plaintext);
                string decrypted = reconstructed.Decrypt(encrypted);

                return new object[] { reconstructed.PublicKeyPem, originalKeyPair.PublicKeyPem, plaintext, decrypted };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            string reconstructedPub = (string)results[0];
            string originalPub = (string)results[1];
            string plaintext = (string)results[2];
            string decrypted = (string)results[3];
            because.ItsTrue("public keys match", reconstructedPub.Equals(originalPub));
            because.ItsTrue("decrypted matches original", plaintext.Equals(decrypted));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void UseNamedKeyExecutesActionWithLoadedKey()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("uses named key to execute action",
            () => CreateStorage(nameof(UseNamedKeyExecutesActionWithLoadedKey)),
            (storage) =>
            {
                using RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
                storage.SaveNamedKey("actionKey", keyPair.Pem);

                string plaintext = "test for use named key";
                string encrypted = new RsaPublicKey(keyPair.PublicKeyPem).Encrypt(plaintext);

                bool actionCalled = false;
                string? decrypted = null;

                storage.UseNamedKey("actionKey", (privateKey) =>
                {
                    actionCalled = true;
                    using RsaPublicPrivateKeyPair loadedPair = new RsaPublicPrivateKeyPair(privateKey.Pem);
                    decrypted = loadedPair.Decrypt(encrypted);
                });

                return new object[] { actionCalled, plaintext, decrypted! };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            bool actionCalled = (bool)results[0];
            string plaintext = (string)results[1];
            string decrypted = (string)results[2];
            because.ItsTrue("action was called", actionCalled);
            because.ItsTrue("decrypted matches original", plaintext.Equals(decrypted));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetNamedKeyReturnsNullForNonExistentKey()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("returns null for non-existent key",
            () => CreateStorage(nameof(GetNamedKeyReturnsNullForNonExistentKey)),
            (storage) =>
            {
                byte[]? result = storage.GetNamedKey("nonExistentKey");
                return result == null;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void UseNamedKeyDoesNothingForNonExistentKey()
    {
        When.A<RsaPrivateKeyOpaqueStorage>("does nothing for non-existent key",
            () => CreateStorage(nameof(UseNamedKeyDoesNothingForNonExistentKey)),
            (storage) =>
            {
                bool actionCalled = false;
                storage.UseNamedKey("nonExistentKey", (privateKey) =>
                {
                    actionCalled = true;
                });
                return actionCalled;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("action was not called", !(bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
