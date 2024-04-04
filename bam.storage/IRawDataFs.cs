namespace Bam.Storage;

public interface IRawDataFs
{
    string Root { get; }
    string GetHashPath(string dataHash);
    string GetIdPath(ulong hashId);
    
    
}