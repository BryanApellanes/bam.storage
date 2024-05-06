using System.Text;
using Bam.Net;

namespace Bam.Storage;

public class RawDataReference : RawData
{
    public RawDataReference(string hashString) : base(hashString)
    {
    }
}