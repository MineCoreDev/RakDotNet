using System;

namespace RakDotNet.Event.RakNetClientEvents
{
    public class RakNetClientEventArgs : EventArgs
    {
        public RakNetSocket Socket { get; }

        public RakNetClientEventArgs(RakNetSocket socket)
        {
            Socket = socket;
        }
    }
}