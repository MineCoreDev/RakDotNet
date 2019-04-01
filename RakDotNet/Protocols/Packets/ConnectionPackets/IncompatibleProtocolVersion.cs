namespace RakDotNet.Protocols.Packets.ConnectionPackets
{
    public class IncompatibleProtocolVersion : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.INCOMPATIBLE_PROTOCOL_VERSION;

        public byte NetworkProtocol { get; set; }
        public long ServerGuid { get; set; }

        public override void EncodePayload()
        {
            WriteByte(NetworkProtocol);
            WriteMagic();
            WriteLong(ServerGuid);
        }

        public override void DecodePayload()
        {
            NetworkProtocol = ReadByte();
            CheckMagic();
            ServerGuid = ReadLong();
        }
    }
}