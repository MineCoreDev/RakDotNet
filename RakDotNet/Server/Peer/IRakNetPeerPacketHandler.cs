using System.Net;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;

namespace RakDotNet.Server.Peer
{
    public interface IRakNetPeerPacketHandler
    {
        void HandlePeerPacket(RakNetPacket packet);
        void HandleEncapsulatedPacket(EncapsulatedPacket packet);
    }
}