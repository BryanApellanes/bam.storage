# bam.storage

A filesystem-backed storage abstraction providing content-addressable raw data storage, slotted file storage, and key-value pair persistence.

## Overview

bam.storage defines a layered storage model centered around three core concepts: **storage holders** (directories), **storage slots** (files within those directories), and **raw data** (byte arrays with computed SHA-256 hashes). Data is stored on disk using a content-addressable scheme where the hash of the data determines its storage path, with the hex string split into two-character segments to create a balanced directory tree.

The library provides both low-level raw storage (`IRawStorage` / `FsRawStorage`) for simple hash-addressed save/load operations, and higher-level slotted storage (`ISlottedStorage` / `FsSlottedStorage`) that adds named relative paths, explicit slot management, and convenience overloads. On top of slotted storage, `FsKeyValuePairStorage` implements a key-value store where keys are hashed to determine file locations and values are stored as raw bytes.

Additionally, the project includes `MultiProcessData`, an abstract class that supports multi-process safe reads and writes using file-based locking with configurable timeouts and retry intervals. Utility classes such as `FsSlotReader` provide typed deserialization from storage slots, supporting primitive types like `bool`, `int`, `long`, `decimal`, `DateTime`, and `byte[]`.

## Key Classes

| Class | Description |
|-------|-------------|
| `RawData` | Wraps a `byte[]` value with automatic SHA-256 hashing and hex string computation. Implements `IRawData`. |
| `FsStorageHolder` | Represents a directory on the filesystem. Provides implicit conversions to/from `string` and `DirectoryInfo`. |
| `DirectoryStorageHolder` | Extends `FsStorageHolder` with static singletons for the working directory and user profile data directory. |
| `RootStorageHolder` | A specialized `DirectoryStorageHolder` implementing `IRootStorageHolder` for use as a storage root. |
| `StorageSlot` | Abstract base for file slots. Computes `FullName` from holder + relative path. Reads data lazily from disk. |
| `FsStorageSlot` | Concrete filesystem slot that writes bytes to disk, creating parent directories as needed. Provides `GetSegmentedPathStorageSlot` for content-addressable path construction. |
| `FsRawStorage` | Content-addressable storage: saves data to a path derived from its hash, loads by hash hex string. |
| `SlottedStorage` | Abstract base class for `ISlottedStorage` implementations. |
| `FsSlottedStorage` | Full-featured filesystem slotted storage with save/load by slot, relative path, hash, or raw bytes. |
| `FsKeyValuePairStorage` | Key-value pair storage backed by `FsSlottedStorage`. Keys are hashed to determine storage slots. |
| `FsSlotReader` | Typed reader that deserializes storage slot contents to primitives (`bool`, `int`, `long`, `decimal`, `DateTime`, `string`, `byte[]`). |
| `MultiProcessData` | Abstract class providing multi-process safe file read/write with lock files and configurable timeout. |
| `DataTypeTranslator` | Maps between CLR types, `DataTypes` enum values, and database type name strings. |
| `HolderInfo` | Lightweight `IStorageHolder` wrapping a full slot path, extracting the directory as the holder. |
| `SlotInfo` | Wrapper around `IStorageSlot` that normalizes path resolution. |
| `StorageSlotExtensions` | Extension methods for `IStorageSlot`: `GetNormalizedSlotInfo`, `GetName`, `GetFullPath`. |
| `RawDataExtensions` | Extension method `ToObject<T>()` to deserialize `IRawData` from JSON. |

## Dependencies

**Project References:**
- `bam.base` -- core framework utilities (hashing, serialization, argument validation, extensions)
- `bam.configuration` -- configuration and profile path resolution (`BamProfile`, `BamDir`)

**Package References:**
- None (pure .NET 10 library)

## Usage Examples

### Content-addressable raw storage

```csharp
using Bam.Storage;

// Create raw storage rooted at a specific directory
var storage = new FsRawStorage("/path/to/storage");

// Save data -- stored at a path derived from its SHA-256 hash
IRawData data = new RawData("hello world");
IStorageSlot slot = storage.Save(data);

// Load data back by hash
IRawData loaded = storage.LoadHashHexString(data.HashHexString);
string text = loaded.ToString(); // "hello world"
```

### Slotted storage with named paths

```csharp
using Bam.Storage;

var storage = new FsSlottedStorage("/path/to/storage");

// Save to a named relative path
storage.Save("config/settings.dat", new RawData("{\"key\": \"value\"}"));

// Load from the same path
IRawData loaded = storage.Load("config/settings.dat");

// Save by content hash (content-addressable)
RawData content = new RawData("some content");
IStorageSlot slot = storage.Save(content);
IRawData retrieved = storage.LoadHashHexString(content.HashHexString);
```

### Key-value pair storage

```csharp
using Bam.Storage;

var kvStore = new FsKeyValuePairStorage("/path/to/kvstore");

// Save a key-value pair
IKeyValuePairSaveResult result = kvStore.Save("myKey", "myValue");
if (result.Success)
{
    // Retrieve the value
    IKeyValuePair pair = kvStore.Get("myKey");
    string value = System.Text.Encoding.UTF8.GetString(pair.Value);
}
```

## Known Gaps / Not Yet Implemented

- `IStorageSearch` is an empty interface with no members or implementations.
- `IRawDataStorageSaveResult` and `IRawDataStorageLoadResult` have concrete classes (`FsRawDataDataStorageSaveResult`, `FsRawDataStorageLoadResult`) but they are not used by the main `FsRawStorage` or `FsSlottedStorage` save/load methods, which return `IStorageSlot` or `IRawData` directly.
- `FsStorageSlot` contains a commented-out duplicate of `GetSegmentedPathStorageSlot` (the active implementation is in the base `StorageSlot` class).
- `KeyValuePairData` in the `Data` namespace implements `IKeyValuePair` but has a commented-out `AuditRepoData` base class, suggesting planned but unfinished integration with the data repository layer.
