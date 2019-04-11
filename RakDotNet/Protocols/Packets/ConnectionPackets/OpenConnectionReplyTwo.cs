using System.Net;

namespace RakDotNet.Protocols.Packets.ConnectionPackets
{
    public class OpenConnectionReplyTwo : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.OPEN_CONNECTION_REPLY_2;

        public long ServerGuid { get; set; }
        public ushort MtuSize { get; set; }
        public bool EncryptionEnabled { get; set; }

        public override void EncodePayload()
        {
            WriteMagic();
            WriteLong(ServerGuid);
            WriteUShort(MtuSize);
            WriteBoolean(EncryptionEnabled);
        }

        public override void DecodePayload()
        {
            CheckMagic();
            ServerGuid = ReadLong();
            MtuSize = ReadUShort();
            EncryptionEnabled = ReadBoolean();
        }
    }
}