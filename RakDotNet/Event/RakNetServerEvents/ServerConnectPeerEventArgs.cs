using RakDotNet.Server;
using RakDotNet.Server.Peer;

namespace RakDotNet.Event.RakNetServerEvents
{
    public class ServerConnectPeerEventArgs : RakNetServerEventArgs
    {
        public RakNetPeer Peer { get; }

        public ServerConnectPeerEventArgs(RakNetServer server, RakNetPeer peer) : base(server)
        {
            Peer = peer;
        }
    }
}