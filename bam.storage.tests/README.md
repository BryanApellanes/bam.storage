# bam.storage.tests

Integration and unit tests for the bam.storage and bam.storage.encryption libraries, verifying filesystem storage, content-addressable persistence, and encrypted opaque storage operations.

## Overview

bam.storage.tests is a console-based test project that uses the bam.test framework's menu-driven test runner (`BamConsoleContext.StaticMain`). Tests are organized into `Unit` and `Integration` directories. The unit tests cover basic filesystem slotted storage operations, while the integration tests exercise the full round-trip of saving and loading data through raw storage, opaque (encrypted) raw storage, opaque slotted storage, and encrypted key-value pair storage.

Each test class extends `UnitTestMenuContainer` and is decorated with `[UnitTestMenu]`. Individual test methods use `[UnitTest]` and the fluent `When.A<T>()` API to set up, execute, and assert results. The tests validate that data can be saved and retrieved identically through each storage layer, including the encryption/decryption round-trip for opaque storage variants.

The project references both bam.storage and bam.storage.encryption, providing coverage across the plain and encrypted storage APIs. It also includes a `TestStorageData` class used as a test fixture with typical property types (`string`, `int`, `long`, `DateTime`).

## Key Classes

| Class | Description |
|-------|-------------|
| `Program` | Entry point that delegates to `BamConsoleContext.StaticMain` for menu-driven test execution. |
| `FsStorageShould` (Unit) | Tests that `FsSlottedStorage` saves a file to the expected filesystem path. |
| `FsRawStorageShould` (Integration) | Tests that `FsRawStorage` can save raw data and retrieve it both from the returned slot and by hash hex string lookup. |
| `OpaqueFsRawStorageShould` (Integration) | Tests that `OpaqueFsRawStorage` encrypts data on save and decrypts correctly on load by hash. |
| `OpaqueFsObjectStorageShould` (Integration) | Tests that `OpaqueFsSlottedStorage` round-trips data through encrypted slotted storage using dependency-injected `IAesKeySource` and `IHmacKeyProvider`. |
| `DataFolderKeyValuePairStorageShould` (Integration) | Tests that `DataFolderOpaqueFsKeyValuePairStorage` can save and retrieve an encrypted key-value pair using system keys. |
| `SystemKeyValuePairStorageShould` (Integration) | Tests that `SystemFsKeyValuePairStorage` can save and retrieve an encrypted key-value pair from the vault.sys directory. |
| `TestStorageData` | Simple POCO with `Name`, `IntProperty`, `StringProperty`, `LongProperty`, and `DateTimeProperty` used as a test data fixture. |

## Dependencies

**Project References:**
- `bam.test` -- test framework providing `UnitTestMenuContainer`, `[UnitTestMenu]`, `[UnitTest]`, and `When.A<T>()` fluent API
- `bam.storage` -- the storage library under test
- `bam.storage.encryption` -- the encrypted storage library under test

**Package References:**
- None

## Usage Examples

### Running all tests

```bash
dotnet run --project submodules/bam.storage/bam.storage.tests/bam.storage.tests.csproj -- --ut
```

Note: Use `--ut` (not `/ut`) when running from Git Bash, as Git Bash rewrites `/ut` to a filesystem path.

### Running from the interactive menu

```bash
dotnet run --project submodules/bam.storage/bam.storage.tests/bam.storage.tests.csproj
```

This launches the menu-driven test runner where individual test classes and methods can be selected interactively.

### Test structure example

```csharp
[UnitTestMenu("OpaqueFsRawStorageShould")]
public class OpaqueFsRawStorageShould : UnitTestMenuContainer
{
    [UnitTest]
    public void SaveAndRetrieveRawData()
    {
        string testData = "this is test data";

        When.A<OpaqueFsRawStorage>("saves and retrieves encrypted raw data",
            () => new OpaqueFsRawStorage(new AesKey(), new HmacKeyProvider(), "test_path"),
            (storage) =>
            {
                IRawData data = new RawData(testData);
                IStorageSlot slot = storage.Save(data);
                IRawData loaded = storage.LoadHashHexString(data.HashHexString);
                return Encoding.UTF8.GetString(loaded.Value);
            })
        .TheTest
        .ShouldPass(because =>
        {
            string retrieved = (string)because.Result;
            because.ItsTrue("retrieved equals original", testData.Equals(retrieved));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
```

## Known Gaps / Not Yet Implemented

- No tests exist for `FsSlotReader` (typed deserialization from storage slots).
- No tests exist for `MultiProcessData` (multi-process locking and concurrent access).
- No tests exist for `FsKeyValuePairStorage` in its non-encrypted form; only the encrypted variants (`DataFolderOpaqueFsKeyValuePairStorage` and `SystemFsKeyValuePairStorage`) are tested.
- No negative/error-path tests (e.g., loading non-existent data, permission failures, lock contention).
