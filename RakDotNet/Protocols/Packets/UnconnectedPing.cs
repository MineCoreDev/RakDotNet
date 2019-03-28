using System;

namespace RakDotNet.Protocols.Packets
{
    public class UnconnectedPing : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.UNCONNECTED_PING;

        public TimeSpan TimeStamp { get; set; }
        public long PingId { get; set; }

        public override void EncodePayload()
        {
            WriteLong(TimeStamp.Milliseconds);
            WriteMagic();
            WriteLong(PingId);
        }

        public override void DecodePayload()
        {
            TimeStamp = TimeSpan.FromMilliseconds(ReadLong());
            CheckMagic();
            PingId = ReadLong();
        }
    }
}