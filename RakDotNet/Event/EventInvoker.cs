using System;

namespace RakDotNet.Event
{
    public static class EventInvoker
    {
        public static void Invoke<T>(this T args, object sender, EventHandler<T> handler) where T : EventArgs
        {
            handler?.Invoke(sender, args);
        }
    }
}