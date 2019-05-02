using RakDotNet.Protocols.Packets;
using RakDotNet.Server.Peer;

namespace RakDotNet.Event.RakNetPeerEvents
{
    public class PeerHandlePacketEventArgs : RakNetPeerEventArgs
    {
        public RakNetPacket Packet { get; }

        public PeerHandlePacketEventArgs(RakNetPeer peer, RakNetPacket packet) : base(peer)
        {
            Packet = packet;
        }
    }
}