using RakDotNet.Server;

namespace RakDotNet.Event.RakNetServerEvents
{
    public class ServerStopWorkerEventArgs : RakNetServerEventArgs
    {
        public ServerStopWorkerEventArgs(RakNetServer server) : base(server)
        {
        }
    }
}