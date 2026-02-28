using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Integration;

[UnitTestMenu("NamedKeyUsageServiceShould")]
public class NamedKeyUsageServiceShould : UnitTestMenuContainer
{
    private static (NamedKeyUsageService Service, RsaPrivateKeyOpaqueStorage Storage) CreateServiceAndStorage(string testName)
    {
        OpaqueFsKeyValuePairStorage opaqueKvStorage = new OpaqueFsKeyValuePairStorage(
            new FsSlottedStorage($"NamedKeyUsageServiceShould_{testName}"),
            new AesKey(),
            new HmacKeyProvider()
        );
        RsaPrivateKeyOpaqueStorage storage = new RsaPrivateKeyOpaqueStorage(opaqueKvStorage);
        NamedKeyUsageService service = new NamedKeyUsageService(storage, new RsaProtectedKeyUsageContextFactory());
        return (service, storage);
    }

    [UnitTest]
    public void UseNamedKeyExecutesActionWithStoredKey()
    {
        var (service, storage) = CreateServiceAndStorage(nameof(UseNamedKeyExecutesActionWithStoredKey));

        When.A<NamedKeyUsageService>("executes action with stored key",
            () => service,
            (svc) =>
            {
                using RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair(RsaKeyLength._2048);
                storage.SaveNamedKey("serviceKey", keyPair.Pem);

                string plaintext = "end-to-end service test";
                string encrypted = new RsaPublicKey(keyPair.PublicKeyPem).Encrypt(plaintext);

                bool actionCalled = false;
                string? decrypted = null;

                svc.UseNamedKey("serviceKey", (privateKey) =>
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
    public void UseNamedKeyDoesNothingForMissingKey()
    {
        When.A<NamedKeyUsageService>("does nothing for missing key",
            () => CreateServiceAndStorage(nameof(UseNamedKeyDoesNothingForMissingKey)).Service,
            (service) =>
            {
                bool actionCalled = false;
                service.UseNamedKey("missingKey", (privateKey) =>
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
