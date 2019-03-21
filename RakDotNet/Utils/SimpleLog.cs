using System.Diagnostics;

namespace RakDotNet.Utils
{
    public struct SimpleLog : ILog
    {
        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }

        public SimpleLog(string type, string message, string stackTrace)
        {
            Type = type;
            Message = message;
            StackTrace = stackTrace;
        }
    }
}