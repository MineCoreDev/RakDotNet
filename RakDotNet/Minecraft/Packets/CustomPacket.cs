using System;
using System.Collections.Generic;
using BinaryIO;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;

namespace RakDotNet.Minecraft.Packets
{
    public class CustomPacket : RakNetPacket
    {
        private byte _packetId = MinecraftServer.CUSTOM_PACKET_0;
        public override byte PacketId => _packetId;

        public uint SequenceId { get; set; }
        public EncapsulatedPacket[] Packets { get; set; }

        public CustomPacket()
        {
        }

        public CustomPacket(byte packetId)
        {
            if (packetId <= 0x80 && packetId >= 0x8f)
                _packetId = packetId;

            throw new ArgumentOutOfRangeException(nameof(packetId));
        }

        public override void DecodeHeader()
        {
            _packetId = ReadByte();
        }

        public override void EncodePayload()
        {
            WriteTriad(SequenceId, ByteOrder.Little);

            for (int i = 0; i < Packets.Length; i++)
            {
                EncapsulatedPacket packet = Packets[i];
                packet.WriteToCustomPacket(this);
            }
        }

        public override void DecodePayload()
        {
            SequenceId = ReadTriad(ByteOrder.Little);

            List<EncapsulatedPacket> packets = new List<EncapsulatedPacket>();
            while (!IsEndOfStream())
            {
                packets.Add(new EncapsulatedPacket(this));
            }

            Packets = packets.ToArray();
        }
    }
}