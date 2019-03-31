namespace RakDotNet.Protocols.Packets.Minecraft.PingPackets
{
    public class UnconnectedPingOpenConnections : UnconnectedPong
    {
        public override byte PacketId => PacketIdentifier.UNCONNECTED_PING_OPEN_CONNECTIONS;
    }
}