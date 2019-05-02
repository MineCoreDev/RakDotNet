using RakDotNet.Server;

namespace RakDotNet.Event.RakNetServerEvents
{
    public class ServerStartWorkerEventArgs : RakNetServerEventArgs
    {
        public ServerStartWorkerEventArgs(RakNetServer server) : base(server)
        {
        }
    }
}