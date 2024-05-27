using Bam;

namespace Bam.Storage;

public static class IResultExtensions
{
    public static void SetMessage(this IResult result, Exception ex)
    {
        switch (ProcessMode.Current.Mode)
        {
            case ProcessModes.Dev:
            case ProcessModes.Test:
                result.Message = ex.GetMessageAndStackTrace();
                break;
            case ProcessModes.Prod:
                result.Message = ex.Message;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}