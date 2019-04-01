namespace RakDotNet.Protocols.Packets.ConnectionPackets
{
    public class OpenConnectionReplyOne : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.OPEN_CONNECTION_REPLY_1;

        public long ServerGuid { get; set; }
        public bool UseSecurity { get; set; }
        public ushort MtuSize { get; set; }

        public override void EncodePayload()
        {
            WriteMagic();
            WriteLong(ServerGuid);
            WriteBoolean(UseSecurity);
            WriteUShort(MtuSize);
        }

        public override void DecodePayload()
        {
            CheckMagic();
            ServerGuid = ReadLong();
            UseSecurity = ReadBoolean();
            MtuSize = ReadUShort();
        }
    }
}