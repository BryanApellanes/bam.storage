using System.Text;
using Bam;

namespace Bam.Storage;

public class RawDataReference : RawData
{
    public RawDataReference(string hashString) : base(hashString)
    {
    }
}