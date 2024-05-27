using Bam;

namespace Bam.Storage;

public static class RawDataExtensions
{
    public static T ToObject<T>(this IRawData rawData)
    {
        return rawData.ToString().FromJson<T>();
    }
}