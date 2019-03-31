using System;

namespace RakDotNet.Protocols.Packets.Minecraft.PingPackets
{
    public class UnconnectedPing : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.UNCONNECTED_PING;

        public TimeSpan TimeStamp { get; set; }
        public long PingId { get; set; }

        public override void EncodePayload()
        {
            WriteLong((long) TimeStamp.TotalMilliseconds);
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