using System;
using System.Diagnostics;

namespace RakDotNet.Utils
{
    public static class Logger
    {
        public static Action<ILog> PrintCallBack { private get; set; } = log =>
        {
            Console.WriteLine($"[{log.Type}] {log.Message}");
        };

        internal static void Debug(object message)
        {
            PrintCallBack(new SimpleLog("Debug", message.ToString(), Environment.StackTrace));
        }

        internal static void Info(object message)
        {
            PrintCallBack(new SimpleLog("Info", message.ToString(), Environment.StackTrace));
        }

        internal static void Warn(object message)
        {
            PrintCallBack(new SimpleLog("Warn", message.ToString(), Environment.StackTrace));
        }

        internal static void Error(object message)
        {
            PrintCallBack(new SimpleLog("Error", message.ToString(), Environment.StackTrace));
        }
    }
}