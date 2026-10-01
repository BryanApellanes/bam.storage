using System.Text;
using System.Text.RegularExpressions;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;
using Bam.Test;

namespace Bam.Application.Unit;

/// <summary>
/// Pins bam.storage#1 at the key-value layer: transformed keys must be hex so the segmented path they produce
/// can never contain a directory separator, and every saved pair must land under the configured root.
/// </summary>
[UnitTestMenu("OpaqueFsKeyValuePairStorageShould")]
public class OpaqueFsKeyValuePairStorageShould : UnitTestMenuContainer
{
    private static readonly Regex LowerHex = new Regex("^[0-9a-f]+$", RegexOptions.Compiled);

    [UnitTest]
    public void TransformKeysToHex()
    {
        string root = TestRoot(nameof(TransformKeysToHex));

        When.A<ExposedOpaqueFsKeyValuePairStorage>("transforms keys into hex-only identifiers",
            () => Create(root),
            (storage) =>
            {
                List<string> transformed = new List<string>();
                for (int i = 0; i < 64; i++)
                {
                    transformed.Add(storage.Transform(32.RandomLetters()));
                }
                return transformed;
            })
        .TheTest
        .ShouldPass<List<string>>((because, transformedKeys) =>
        {
            because.ItsTrue("every transformed key is lower-case hex", transformedKeys.TrueForAll(key => LowerHex.IsMatch(key)));
            because.ItsTrue("no transformed key contains a path separator", transformedKeys.TrueForAll(key => key.IndexOfAny(new char[] { '/', '\\', '+', '=' }) < 0));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SaveEveryPairUnderTheRoot()
    {
        string root = TestRoot(nameof(SaveEveryPairUnderTheRoot));
        if (Directory.Exists(root))
        {
            Directory.Delete(root, true);
        }

        When.A<ExposedOpaqueFsKeyValuePairStorage>("saves and retrieves pairs without leaving the root",
            () => Create(root),
            (storage) =>
            {
                int saved = 0;
                int roundTripped = 0;
                for (int i = 0; i < 32; i++)
                {
                    string key = 24.RandomLetters();
                    string value = 16.RandomLetters();
                    storage.Save(key, value);
                    saved++;
                    IKeyValuePair retrieved = storage.Get(key);
                    if (value.Equals(Encoding.UTF8.GetString(retrieved.Value)))
                    {
                        roundTripped++;
                    }
                }
                int dataFiles = Directory.EnumerateFiles(root, SegmentedPath.DataFileName, SearchOption.AllDirectories).Count();
                return new SaveOutcome(saved, roundTripped, dataFiles);
            })
        .TheTest
        .ShouldPass<SaveOutcome>((because, outcome) =>
        {
            because.ItsTrue("every pair round-tripped", outcome.Saved == outcome.RoundTripped);
            because.ItsTrue("one data file per pair exists under the root", outcome.DataFilesUnderRoot == outcome.Saved);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private ExposedOpaqueFsKeyValuePairStorage Create(string root)
    {
        ServiceRegistry registry = Configure(svcReg =>
        {
            svcReg.For<IAesKeySource>().UseSingleton(new AesKey());
            svcReg.For<IHmacKeyProvider>().Use<HmacKeyProvider>();
            svcReg.For<FsSlottedStorage>().UseSingleton(new FsSlottedStorage(root));
        });
        return registry.Get<ExposedOpaqueFsKeyValuePairStorage>();
    }

    private static string TestRoot(string testName)
    {
        return Path.Combine(Environment.CurrentDirectory, nameof(OpaqueFsKeyValuePairStorageShould), testName);
    }

    /// <summary>
    /// Exposes the protected key transform so the test can inspect the identifier that becomes the storage path.
    /// </summary>
    public class ExposedOpaqueFsKeyValuePairStorage : OpaqueFsKeyValuePairStorage
    {
        public ExposedOpaqueFsKeyValuePairStorage(FsSlottedStorage slottedStorage, IAesKeySource aesKeySource, IHmacKeyProvider hmacKeyProvider)
            : base(slottedStorage, aesKeySource, hmacKeyProvider)
        {
        }

        public string Transform(string key)
        {
            return TransformKey(key);
        }
    }

    private sealed record SaveOutcome(int Saved, int RoundTripped, int DataFilesUnderRoot);
}
