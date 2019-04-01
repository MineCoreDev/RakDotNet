using RakDotNet.IO;
using RakDotNet.Protocols.Packets;

namespace RakDotNet.Minecraft.Packets
{
    public class EncapsulatedPacket
    {
        private const int FLAG_RELIABILITY_INDEX = 5;
        private const byte FLAG_RELIABILITY = 0b11100000;
        private const byte FLAG_SPLIT = 0b00010000;

        public Reliability Reliability { get; set; }
        public bool Split { get; set; }

        public uint MessageIndex { get; set; }

        public uint OrderIndex { get; set; }
        public byte OrderChannel { get; set; }

        public int SplitCount { get; set; }
        public ushort SplitId { get; set; }
        public int SplitIndex { get; set; }

        public byte[] Payload { get; set; }

        public EncapsulatedPacket(CustomPacket packet)
        {
            byte flags = packet.ReadByte();
            Reliability = (Reliability) ((flags & FLAG_RELIABILITY) >> FLAG_RELIABILITY_INDEX);
            Split = (flags & FLAG_SPLIT) > 0;

            int length = packet.ReadUShort() / 8;
            if (Reliability.IsReliable())
            {
                MessageIndex = packet.ReadTriad(ByteOrder.Little);
            }

            if (Reliability.IsOrdered() || Reliability.IsSequenced())
            {
                OrderIndex = packet.ReadTriad(ByteOrder.Little);
                OrderChannel = packet.ReadByte();
            }

            if (Split)
            {
                SplitCount = packet.ReadInt();
                SplitId = packet.ReadUShort();
                SplitIndex = packet.ReadInt();
            }

            Payload = packet.ReadBytes(length);
        }

        public int GetPacketSize()
        {
            return 1 + 2 + (Reliability.IsReliable() ? 3 : 0) +
                   (Reliability.IsOrdered() || Reliability.IsSequenced() ? 4 : 0) +
                   (Split ? 10 : 0);
        }
    }
}