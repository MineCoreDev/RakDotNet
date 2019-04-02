using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;

namespace RakDotNet.Minecraft.Packets.Acknowledge
{
    public class NackPacket : AckPacket
    {
        public override byte PacketId => MinecraftServer.NACK;
    }
}