namespace RakDotNet.Protocols.Packets.Minecraft.ConnectionPackets
{
    public class OpenConnectionReplyOne : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.OPEN_CONNECTION_REPLY_1;

        public override void EncodePayload()
        {
        }

        public override void DecodePayload()
        {
        }
    }
}