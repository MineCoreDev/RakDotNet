using RakDotNet.Protocols.Packets;

namespace RakDotNet.Minecraft.Packets
{
    public class BatchPacket : RakNetPacket
    {
        public override byte PacketId => MinecraftServer.BATCH_PACKET;

        public byte[] Payload;

        public override void EncodePayload()
        {
            WriteBytes(Payload);
        }

        public override void DecodePayload()
        {
            Payload = ReadBytes();
        }
    }
}