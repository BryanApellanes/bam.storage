using Bam.Data.Repositories;

namespace Bam.Storage.Data;

public class KeyValuePairData : AuditRepoData, IKeyValuePair
{
    public string Key { get; set; }
    public byte[] Value { get; set; }
}