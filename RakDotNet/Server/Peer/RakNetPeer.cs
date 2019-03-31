using System.Net;

namespace RakDotNet.Server.Peer
{
    public class RakNetPeer
    {
        public IPEndPoint PeerEndPoint { get; }
        public RakNetServer Server { get; private set; }

        public RakNetPeer(IPEndPoint endPoint)
        {
            PeerEndPoint = endPoint;
        }

        public virtual void Connect(RakNetServer server)
        {
            Server = server;
        }

        public virtual void Disconnect()
        {
            Server.Disconnect(PeerEndPoint);
        }
    }
}