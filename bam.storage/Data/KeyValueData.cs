using Bam.Data.Repositories;

namespace Bam.Storage.Data;

public class KeyValueData : AuditRepoData, IKeyValue
{
    public string Key { get; set; }
    public byte[] Value { get; set; }
}