using RakDotNet.Server;
using RakDotNet.Server.Peer;

namespace RakDotNet.Event.RakNetServerEvents
{
    public class ServerDisconnectPeerEventArgs : RakNetServerEventArgs
    {
        public RakNetPeer Peer { get; }

        public ServerDisconnectPeerEventArgs(RakNetServer server, RakNetPeer peer) : base(server)
        {
            Peer = peer;
        }
    }
}