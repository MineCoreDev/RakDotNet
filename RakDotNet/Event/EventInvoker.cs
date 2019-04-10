using System;

namespace RakDotNet.Event
{
    public static class EventInvoker
    {
        public static void Invoke<T>(this T args, object sender, EventHandler<T> handler) where T : EventArgs
        {
            handler?.Invoke(sender, args);
        }

        public static bool CancelableInvoke<T>(this T args, object sender, EventHandler<T> handler)
            where T : ICancelableEvent
        {
            handler?.Invoke(sender, args);

            return args.IsCanceled;
        }
    }
}