using System.Net;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Minecraft.Packets.Acknowledge;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Server.Peer;
using RakDotNet.Utils;

namespace RakDotNet.Minecraft
{
    public class MinecraftPeer : RakNetPeer
    {
        public MinecraftPeer(IPEndPoint endPoint, long clientId, ushort mtuSize) : base(endPoint, clientId, mtuSize)
        {
        }

        public override void HandlePeerPacket(RakNetPacket packet)
        {
            if (packet is CustomPacket customPacket)
                HandleCustomHandle(customPacket);
        }

        public void HandleCustomHandle(CustomPacket packet)
        {
            SendAck(packet.SequenceId);

            uint handleSeq = packet.SequenceId + 1;
            uint diff = handleSeq - ReceiveSequenceNumber;
            if (ReceiveSequenceNumber < handleSeq)
            {
                if (diff != 1)
                    for (uint i = ReceiveSequenceNumber; i < ReceiveSequenceNumber + diff - 1; i++)
                        SendNack(i - 1);

                ReceiveSequenceNumber = handleSeq;

                for (int i = 0; i < packet.Packets.Length; i++)
                {
                    HandleEncapsulatedPacket(packet.EndPoint, packet.Packets[i]);
                }
            }
        }

        public override void HandleEncapsulatedPacket(IPEndPoint endPoint, EncapsulatedPacket packet)
        {
            byte[] buffer = packet.Payload;
            RakNetPacket pk = Server.Client.PacketIdentifier.GetPacketFormId(buffer[0]);
            pk.SetBuffer(buffer);
        }

        public void SendAck(uint sequenceId)
        {
            AckPacket packet = (AckPacket) Server.Client.PacketIdentifier.GetPacketFormId(MinecraftServer.ACK);
            packet.Records.Add(new Record(sequenceId));
            packet.EndPoint = PeerEndPoint;

            SendPacket(packet);
        }

        public void SendNack(uint sequenceId)
        {
            NackPacket packet = (NackPacket) Server.Client.PacketIdentifier.GetPacketFormId(MinecraftServer.NACK);
            packet.Records.Add(new Record(sequenceId));
            packet.EndPoint = PeerEndPoint;

            SendPacket(packet);
        }
    }
}