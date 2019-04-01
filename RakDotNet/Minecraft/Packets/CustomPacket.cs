using System;
using System.Collections.Generic;
using RakDotNet.IO;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Server;

namespace RakDotNet.Minecraft.Packets
{
    public class CustomPacket : RakNetPacket
    {
        private byte _packetId = MinecraftServer.CUSTOM_PACKET_0;
        public override byte PacketId => _packetId;

        public uint SequenceId { get; set; }
        public EncapsulatedPacket[] Packets { get; }

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
            
            List<EncapsulatedPacket> packets = new List<EncapsulatedPacket>();
            while (!IsEndOfStream())
            {
                packets.Add(new EncapsulatedPacket(this));
            }
        }

        public override void DecodePayload()
        {
            SequenceId = ReadTriad(ByteOrder.Little);
        }
    }
}