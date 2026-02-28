# Decouple Key Storage from Key Usage in RsaPrivateKeyOpaqueStorage

## Status: Implemented

## Summary

Separated three responsibilities in `RsaPrivateKeyOpaqueStorage`: named key persistence, byte-level key transformation, and scoped key usage orchestration.

## Changes

### New files (bam.encryption)
- `INamedKeyStorage.cs` — algorithm-agnostic interface for storing/retrieving named key material
- `IProtectedKeyUsageContextFactory.cs` — factory for creating `ProtectedKeyUsageContext` instances
- `RsaProtectedKeyUsageContextFactory.cs` — RSA implementation of the factory

### New files (bam.storage.encryption)
- `NamedKeyUsageService.cs` — orchestrates key loading + protected usage via composition

### Modified files
- `RsaPrivateKeyOpaqueStorage.cs` — now implements `INamedKeyStorage` + `IRsaPrivateKeyByteWriter` (removed `IRsaPrivateKeyByteReader`, `ReadPrivateKey`, `UseNamedKey`, parameterless ctor)
- `RsaPrivateKeyOpaqueStorageShould.cs` — updated tests to use `RsaPrivateKeyByteReader` and `NamedKeyUsageService`
- `NamedKeyUsageServiceShould.cs` — new test class with 2 end-to-end tests
- `README.md` — updated class table and usage example

### Also affects (bam.encryption, existing, unchanged)
- `RsaPrivateKeyByteReader` — callers now use this directly instead of `RsaPrivateKeyOpaqueStorage.ReadPrivateKey`

## Architecture

```
INamedKeyStorage <── RsaPrivateKeyOpaqueStorage (also: IRsaPrivateKeyByteWriter)
IProtectedKeyUsageContextFactory <── RsaProtectedKeyUsageContextFactory
NamedKeyUsageService (composes INamedKeyStorage + IProtectedKeyUsageContextFactory)
```

## Test Results

15 passed, 0 failed (including 2 new NamedKeyUsageServiceShould tests).
