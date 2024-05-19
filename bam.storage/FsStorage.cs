using Bam.Net;

namespace Bam.Storage;

public class FsStorage : Storage
{
    public static implicit operator DirectoryInfo(FsStorage storage)
    {
        return storage.Directory;
    }
    
    public FsStorage()
    {
        this.Directory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "storage"));
        this.RootHolder = new FsStorageHolder(this.Directory);
    }

    public FsStorage(string path)
    {
        this.Directory = new DirectoryInfo(path);
        this.RootHolder = new FsStorageHolder(this.Directory);
    }
    
    public DirectoryInfo Directory { get; }

    public override IStorageHolder RootHolder { get; }

    public override IStorageSlot Save(IRawData data)
    {
        List<string> parts = new List<string>();
        parts.AddRange(data.HashId.ToString().Split(2));
        parts.Add("dat");
        return Save(Path.Combine(parts.ToArray()), data);
    }

    public override IStorageSlot Save(byte[] data)
    {
        RawData rawData = new RawData(data);
        return Save(rawData);
    }
    
    public override IStorageSlot Save(string relativePath, byte[] data)
    {
        return Save(relativePath, new RawData(data));
    }
    
    public override IStorageSlot Save(string relativePath, IRawData rawData)
    {
        FileInfo fileInfo = new FileInfo(Path.Combine(Directory.FullName, relativePath));
        if (!fileInfo.Directory.Exists)
        {
            fileInfo.Directory.Create();
        }
        this.WriteBytes(fileInfo.FullName, rawData.Value);
        FsStorageSlot slot = new FsStorageSlot(RootHolder, relativePath);
        slot.SetData(rawData);
        return slot;
    } 
    
    public override IRawData Load(string hashIdString)
    {
        string path = GetHashIdPath(hashIdString);
        return new RawData(this.ReadBytes(path));
    }

    public override IRawData Load(ulong hashId)
    {
        string path = GetHashIdPath(hashId);
        return new RawData(this.ReadBytes(path));
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