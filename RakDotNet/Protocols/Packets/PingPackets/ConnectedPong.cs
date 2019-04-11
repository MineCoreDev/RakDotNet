namespace RakDotNet.Protocols.Packets.PingPackets
{
    public class ConnectedPong : ConnectedPing
    {
        public override byte PacketId => PacketIdentifier.CONNECTED_PONG;
    }
}