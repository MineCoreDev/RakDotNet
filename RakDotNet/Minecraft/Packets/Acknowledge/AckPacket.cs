using System.Collections.Generic;
using BinaryIO;
using RakDotNet.Protocols.Packets;

namespace RakDotNet.Minecraft.Packets.Acknowledge
{
    public class AckPacket : RakNetPacket
    {
        public override byte PacketId => MinecraftServer.ACK;

        public List<Record> Records { get; set; } = new List<Record>();

        public override void EncodePayload()
        {
            Records = Record.SortRecords(Records.ToArray());
            WriteUShort((ushort) Records.Count);
            for (int i = 0; i < Records.Count; i++)
            {
                Record record = Records[i];
                if (record.IsRanged())
                {
                    WriteByte(0);
                    WriteTriad(record.StartIndex, ByteOrder.Little);
                    WriteTriad(record.EndIndex, ByteOrder.Little);
                }
                else
                {
                    WriteByte(1);
                    WriteTriad(record.StartIndex, ByteOrder.Little);
                }
            }
        }

        public override void DecodePayload()
        {
            ushort length = ReadUShort();
            for (int i = 0; i < length; i++)
            {
                byte flag = ReadByte();
                if (flag == 1)
                    Records.Add(new Record(ReadTriad(ByteOrder.Little)));
                else
                    Records.Add(new Record(ReadTriad(ByteOrder.Little), ReadTriad(ByteOrder.Little)));
            }
        }
    }
}