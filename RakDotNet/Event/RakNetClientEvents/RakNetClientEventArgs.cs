using System;

namespace RakDotNet.Event.RakNetClientEvents
{
    public class RakNetClientEventArgs : EventArgs
    {
        public RakNetClient Client { get; }

        public RakNetClientEventArgs(RakNetClient client)
        {
            Client = client;
        }
    }
}