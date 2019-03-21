namespace RakDotNet.Utils
{
    public interface ILog
    {
        string Type { get; }
        string Message { get; }
        string StackTrace { get; }
    }
}