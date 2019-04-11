using System;

namespace RakDotNet.Protocols.Packets.PingPackets
{
    public class ConnectedPing : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.CONNECTED_PING;

        public TimeSpan Timestamp { get; set; }

        public override void EncodePayload()
        {
            WriteLong((long) Timestamp.TotalMilliseconds);
        }

        public override void DecodePayload()
        {
            Timestamp = TimeSpan.FromMilliseconds(ReadLong());
        }
    }
}