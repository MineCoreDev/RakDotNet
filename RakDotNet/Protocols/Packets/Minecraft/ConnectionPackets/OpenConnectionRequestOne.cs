namespace RakDotNet.Protocols.Packets.Minecraft.ConnectionPackets
{
    public class OpenConnectionRequestOne : RakNetPacket
    {
        public const ushort MTU_PADDING = 18;

        public override byte PacketId => PacketIdentifier.OPEN_CONNECTION_REQUEST_1;

        public byte Protocol { get; set; }

        public ushort MtuSize { get; set; }

        public override void EncodePayload()
        {
            WriteMagic();
            WriteByte(Protocol);
            WriteBytes(new byte[MtuSize]);
        }

        public override void DecodePayload()
        {
            CheckMagic();
            Protocol = ReadByte();
            MtuSize = (ushort) (Length - MTU_PADDING);
        }
    }
}