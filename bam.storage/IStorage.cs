namespace Bam.Storage;

public interface IStorage
{
    IStorageIdentifier Identifier { get; }
    IRawData Save(IRawData rawData);
    IRawData Save(string path, IRawData rawData);
    IRawData Save(byte[] data);
    IRawData Save(string path, byte[] data);
    IRawData Load(string hash);
    IRawData Load(ulong hashId);
}