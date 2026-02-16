namespace Bam.Storage;

/// <summary>
/// Provides extension methods for <see cref="IResult"/> instances.
/// </summary>
public static class IResultExtensions
{
    /// <summary>
    /// Sets the <see cref="IResult.Message"/> from an exception. In Dev and Test modes, includes the full stack trace; in Prod mode, includes only the exception message.
    /// </summary>
    /// <param name="result">The result whose message will be set.</param>
    /// <param name="ex">The exception to extract the message from.</param>
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