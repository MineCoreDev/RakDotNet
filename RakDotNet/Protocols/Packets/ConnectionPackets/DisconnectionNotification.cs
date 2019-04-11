namespace RakDotNet.Protocols.Packets.ConnectionPackets
{
    public class DisconnectionNotification : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.DISCONNECTION_NOTIFICATION;
    }
}