using System;

namespace RakDotNet.Protocols.Packets.LoginPackets
{
    public class ConnectionRequest : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.CONNECTION_REQUEST;

        public long ClientId { get; set; }
        public TimeSpan Timestamp { get; set; }
        public bool UseSecurity { get; set; }

        public override void EncodePayload()
        {
            WriteLong(ClientId);
            WriteLong((long) Timestamp.TotalMilliseconds);
            WriteBoolean(UseSecurity);
        }

        public override void DecodePayload()
        {
            ClientId = ReadLong();
            Timestamp = TimeSpan.FromMinutes(ReadLong());
            UseSecurity = ReadBoolean();
        }
    }
}