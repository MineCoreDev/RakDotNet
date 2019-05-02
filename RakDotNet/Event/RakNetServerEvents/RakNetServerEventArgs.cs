using System;
using RakDotNet.Server;

namespace RakDotNet.Event.RakNetServerEvents
{
    public class RakNetServerEventArgs : EventArgs
    {
        public RakNetServer Server { get; }

        public RakNetServerEventArgs(RakNetServer server)
        {
            Server = server;
        }
    }
}