using System.Net;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;

namespace RakDotNet.Server.Peer
{
    public interface IRakNetPeerPacketHandler
    {
        void HandleCustomPacket(CustomPacket packet);
        void HandleEncapsulatedPacket(IPEndPoint endPoint, EncapsulatedPacket packet);
    }
}