using System;

namespace RakDotNet.Protocols.Packets.Minecraft.PingPackets
{
    public class UnconnectedPong : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.UNCONNECTED_PONG;

        public TimeSpan TimeStamp { get; set; }
        public long PongId { get; set; }
        public String Identifier;

        public override void EncodePayload()
        {
            WriteLong((long) TimeStamp.TotalMilliseconds);
            WriteLong(PongId);
            WriteMagic();
            WriteStringUtf8(Identifier);
        }

        public override void DecodePayload()
        {
            TimeStamp = TimeSpan.FromMilliseconds(ReadLong());
            PongId = ReadLong();
            CheckMagic();
            Identifier = ReadStringUtf8();
        }
    }
}