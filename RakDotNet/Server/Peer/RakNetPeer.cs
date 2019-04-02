using System;
using System.Net;
using RakDotNet.Minecraft;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Minecraft.Packets.Acknowledge;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;

namespace RakDotNet.Server.Peer
{
    public class RakNetPeer : IRakNetPeerPacketHandler
    {
        public IPEndPoint PeerEndPoint { get; }
        public RakNetServer Server { get; private set; }

        public long ClientId { get; }
        public ushort MtuSize { get; }
        public RakNetPeerState State { get; private set; } = RakNetPeerState.Connected;
        public long TimeOut { get; set; } = 5000;

        public uint SendSequenceNumber { get; private set; }
        public uint ReceiveSequenceNumber { get; private set; }

        public RakNetPeer(IPEndPoint endPoint, long clientId, ushort mtuSize)
        {
            PeerEndPoint = endPoint;
            ClientId = clientId;
            MtuSize = mtuSize;
        }

        public virtual void Connect(RakNetServer server)
        {
            Server = server;
        }

        public virtual void Disconnect()
        {
            Server.Disconnect(PeerEndPoint);
        }

        public virtual void Disconnect(string reason)
        {
            Server.Disconnect(PeerEndPoint);
        }

        public virtual void HandleCustomPacket(CustomPacket packet)
        {
            SendAck(packet.SequenceId);
        }

        public virtual void HandleEncapsulatedPacket(IPEndPoint endPoint, EncapsulatedPacket packet)
        {
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

        public void SendPacket(RakNetPacket packet)
        {
            Server.Client.SendPacket(packet);
        }
    }
}