using Bam.Net;

namespace Bam.Storage;

public class FsStorage : IStorage
{
    public static implicit operator DirectoryInfo(FsStorage storage)
    {
        return storage.Directory;
    }
    
    public FsStorage()
    {
        this.Directory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "storage"));
        this.Identifier = new FsStorageIdentifier(this.Directory);
    }

    public FsStorage(string path)
    {
        this.Directory = new DirectoryInfo(path);
        this.Identifier = new FsStorageIdentifier(this.Directory);
    }
    
    public DirectoryInfo Directory { get; }

    public IStorageIdentifier Identifier { get; }

    public IRawData Save(IRawData data)
    {
        Save(data.Value);
        return data;
    }

    public IRawData Save(string path, IRawData rawData)
    {
        return Save(path, rawData.Value);
    }

    public IRawData Save(byte[] data)
    {
        RawData rawData = new RawData(data);
        Save(Path.Combine(Directory.FullName, rawData.HashId.ToString()), data);
        return rawData;
    }

    public IRawData Save(string path, byte[] data)
    {
        File.WriteAllBytes(path, data);
        return new RawData(data);
    }
    
    public IRawData Load(string hash)
    {
        string path = GetHashPath(hash);
        return new RawData(File.ReadAllBytes(path));
    }

    public IRawData Load(ulong hashId)
    {
        string path = GetHashIdPath(hashId);
        return new RawData(File.ReadAllBytes(path));
    }

    public virtual string GetStoragePath(IRawData data)
    {
        return GetHashIdPath(data.HashId);
    }
    
    public virtual string GetHashPath(string hash)
    {
        return GetHashIdPath(BitConverter.ToUInt64(hash.HashToByteArray(), 0));
    }

    public virtual string GetHashIdPath(ulong hashId)
    {
        return Path.Combine(Directory.FullName, hashId.ToString());
    }
}