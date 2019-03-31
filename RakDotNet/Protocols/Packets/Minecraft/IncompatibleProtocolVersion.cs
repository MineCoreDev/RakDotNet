namespace RakDotNet.Protocols.Packets.Minecraft
{
    public class IncompatibleProtocolVersion : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.INCOMPATIBLE_PROTOCOL_VERSION;

        public byte NetworkProtocol { get; set; }
        public long SererGuid { get; set; }

        public override void EncodePayload()
        {
            WriteByte(NetworkProtocol);
            WriteMagic();
            WriteLong(SererGuid);
        }

        public override void DecodePayload()
        {
            NetworkProtocol = ReadByte();
            CheckMagic();
            SererGuid = ReadLong();
        }
    }
}