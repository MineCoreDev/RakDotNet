using System;
using System.Net;
using RakDotNet.Minecraft;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Minecraft.Packets.Acknowledge;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Utils;

namespace RakDotNet.Server.Peer
{
    public class RakNetPeer : IRakNetPeerPacketHandler
    {
        public IPEndPoint PeerEndPoint { get; }
        public RakNetServer Server { get; private set; }

        public long ClientId { get; }
        public ushort MtuSize { get; }
        public RakNetPeerState State { get; protected set; } = RakNetPeerState.Connected;
        public long TimeOut { get; set; } = 5000;

        public uint SendSequenceNumber { get; protected set; }
        public uint ReceiveSequenceNumber { get; protected set; }

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

        public virtual void HandlePeerPacket(RakNetPacket packet)
        {
        }

        public virtual void HandleEncapsulatedPacket(IPEndPoint endPoint, EncapsulatedPacket packet)
        {
        }

        public void SendPacket(RakNetPacket packet)
        {
            Server.Client.SendPacket(packet);
        }
    }
}