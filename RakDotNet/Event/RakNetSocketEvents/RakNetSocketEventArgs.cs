using System;

namespace RakDotNet.Event.RakNetSocketEvents
{
    public class RakNetSocketEventArgs : EventArgs
    {
        public RakNetSocket Socket { get; }

        public RakNetSocketEventArgs(RakNetSocket socket)
        {
            Socket = socket;
        }
    }
}