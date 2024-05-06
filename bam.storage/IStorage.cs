namespace Bam.Storage;

public interface IStorage
{
    IStorageIdentifier Identifier { get; }
    IRawData Save(IRawData rawData);
    IRawData Save(string relativePath, IRawData rawData);
    IRawData Save(byte[] data);
    IRawData Save(string relativePath, byte[] data);
    IRawData Load(string hashIdString);
    IRawData Load(ulong hashId);
}