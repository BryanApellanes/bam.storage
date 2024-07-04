using Bam;

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
    public override IStorageSlot CurrentSlot { get; set; }

    public override IStorageSlot GetSlot()
    {
        return CurrentSlot ?? GetSlot("dat");
    }

    public override IStorageSlot GetSlot(string relativePath)
    {
        return new FsStorageSlot(RootHolder, relativePath);
    }

    public override IStorageSlot Save(IRawData data)
    {
        return Save(this.CurrentSlot ?? this.GetHashIdSlot(data.HashId), data);
    }

    public override IStorageSlot Save(IStorageSlot slot, IRawData rawData)
    {
        Args.ThrowIfNull(RootHolder, nameof(RootHolder));
        Args.ThrowIf(!slot.StorageHolder.FullName.StartsWith(RootHolder.FullName, StringComparison.InvariantCultureIgnoreCase), $"slot is in {slot.StorageHolder.FullName} not in storage root {RootHolder.FullName}");

        FileInfo fileInfo = new FileInfo(slot.FullName);
        if (!fileInfo.Directory.Exists)
        {
            fileInfo.Directory.Create();
        }
        this.WriteBytes(fileInfo.FullName, rawData.Value);
        slot.SetData(rawData);
        return slot;
    }

    public override IRawData LoadHashString(string hashString)
    {
        return LoadHashId(BitConverter.ToUInt64(hashString.HashToByteArray(), 0));
    }
    
    public override IRawData LoadHashId(ulong hashId)
    {
        return this.LoadSlot(GetHashIdSlot(hashId));
    }
    
    public override IRawData LoadSlot(IStorageSlot slot)
    {
        if (File.Exists(slot.FullName))
        {
            return new RawData(this.ReadBytes(slot.FullName));
        }

        throw new ArgumentException($"slot not found {slot.FullName}");
    }

    public override IStorageSlot Save(byte[] data)
    {
        RawData rawData = new RawData(data);
        return Save(rawData);
    }

    public override IStorageSlot Save(IStorageSlot slot, byte[] data)
    {
        return this.Save(slot, new RawData(data));
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
        IStorageSlot slot = this.GetSlot(relativePath);
        slot.SetData(rawData);
         return slot;
    } 

    public override IRawData Load(string relativePath)
    {
        string path = Path.Combine(this.RootHolder.FullName, relativePath);
        return new RawData(this.ReadBytes(path));
    }

    public virtual string GetStoragePath(IRawData data)
    {
        return GetHashIdPath(data.HashId);
    }
    
    /// <summary>
    /// Gets the path for the specified hash id.
    /// </summary>
    /// <param name="hashString">The string representation of the HashId. </param>
    /// <returns></returns>
    public virtual string GetHashIdPath(string hashString)
    {
        return GetHashIdPath(BitConverter.ToUInt64(hashString.HashToByteArray(), 0));
    }

    public virtual IStorageSlot GetHashStringSlot(string hashString)
    {
        return GetHashIdSlot(BitConverter.ToUInt64(hashString.HashToByteArray(), 0));
    }
    
    public virtual IStorageSlot GetHashIdSlot(ulong hashId)
    {
        List<string> parts = new List<string>();
        parts.AddRange(hashId.ToString().Split(2));
        parts.Add("dat");
        return new FsStorageSlot(RootHolder, Path.Combine(parts.ToArray()));
    }
    
    public virtual string GetHashIdPath(ulong hashId)
    {
        return GetHashIdSlot(hashId).FullName;
    }
}