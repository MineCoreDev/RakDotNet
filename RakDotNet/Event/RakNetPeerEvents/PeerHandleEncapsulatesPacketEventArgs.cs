using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Server.Peer;

namespace RakDotNet.Event.RakNetPeerEvents
{
    public class PeerHandleEncapsulatesPacketEventArgs : RakNetPeerEventArgs
    {
        public EncapsulatedPacket Packet { get; }

        public PeerHandleEncapsulatesPacketEventArgs(RakNetPeer peer, EncapsulatedPacket packet) : base(peer)
        {
            Packet = packet;
        }
    }
}