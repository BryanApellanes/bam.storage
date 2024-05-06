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

    public IRawData Save(string relativePath, IRawData rawData)
    {
        return Save(relativePath, rawData.Value);
    }

    public IRawData Save(byte[] data)
    {
        RawData rawData = new RawData(data);
        Save(rawData.HashId.ToString(), data);
        return rawData;
    }

    public IRawData Save(string relativePath, byte[] data)
    {
        FileInfo fileInfo = new FileInfo(Path.Combine(Directory.FullName, relativePath));
        if (!fileInfo.Directory.Exists)
        {
            fileInfo.Directory.Create();
        }
        File.WriteAllBytes(fileInfo.FullName, data);
        return new RawData(data);
    }
    
    public IRawData Load(string hashIdString)
    {
        string path = GetHashIdPath(hashIdString);
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
    
    /// <summary>
    /// Gets the path for the specified hash id.
    /// </summary>
    /// <param name="hashIdString">The string representation of the HashId. </param>
    /// <returns></returns>
    public virtual string GetHashIdPath(string hashIdString)
    {
        return GetHashIdPath(BitConverter.ToUInt64(hashIdString.HashToByteArray(), 0));
    }

    public virtual string GetHashIdPath(ulong hashId)
    {
        List<string> parts = new List<string> { Directory.FullName };
        parts.AddRange(hashId.ToString().Split(2));
        return Path.Combine(parts.ToArray());
    }
}