using System;
using System.Net;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Minecraft.Packets.Acknowledge;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.ConnectionPackets;
using RakDotNet.Protocols.Packets.PingPackets;
using RakDotNet.Server;
using RakDotNet.Server.Peer;
using RakDotNet.Utils;

namespace RakDotNet.Minecraft
{
    public class MinecraftServer : RakNetServer
    {
        public const int CUSTOM_PACKET_0 = 0x80;
        public const int CUSTOM_PACKET_1 = 0x81;
        public const int CUSTOM_PACKET_2 = 0x82;
        public const int CUSTOM_PACKET_3 = 0x83;
        public const int CUSTOM_PACKET_4 = 0x84;
        public const int CUSTOM_PACKET_5 = 0x85;
        public const int CUSTOM_PACKET_6 = 0x86;
        public const int CUSTOM_PACKET_7 = 0x87;
        public const int CUSTOM_PACKET_8 = 0x88;
        public const int CUSTOM_PACKET_9 = 0x89;
        public const int CUSTOM_PACKET_A = 0x8A;
        public const int CUSTOM_PACKET_B = 0x8B;
        public const int CUSTOM_PACKET_C = 0x8C;
        public const int CUSTOM_PACKET_D = 0x8D;
        public const int CUSTOM_PACKET_E = 0x8E;
        public const int CUSTOM_PACKET_F = 0x8F;

        public const int ACK = 0xC0;
        public const int NACK = 0xA0;

        public const int BATCH_PACKET = 0xFE;

        public String ServerListData { get; set; } = String.Empty;

        public MinecraftServer(IPEndPoint endPoint) : base(endPoint)
        {
            RegisterMinecraftDefaults();
        }

        public MinecraftServer(IPEndPoint endPoint, PacketIdentifier identifier) : base(endPoint, identifier)
        {
        }

        public override void HandleRakNetPacket(RakNetPacket packet)
        {
            if (packet is CustomPacket customPacket && IsConnected(packet.EndPoint))
            {
                IPEndPoint endPoint = packet.EndPoint;
                MinecraftPeer peer = GetPeer<MinecraftPeer>(endPoint);
                peer.HandlePeerPacket(customPacket);
            }
            else if (packet is UnconnectedPing unconnectedPing)
            {
                UnconnectedPong pong =
                    (UnconnectedPong) Socket.PacketIdentifier.GetPacketFormId(PacketIdentifier.UNCONNECTED_PONG);
                pong.TimeStamp = TimeSpan.FromMilliseconds(Environment.TickCount);
                pong.PongId = PongId;
                pong.Identifier = ServerListData;
                pong.EndPoint = unconnectedPing.EndPoint;
                Socket.SendPacketAsync(pong);
            }
            else if (packet is OpenConnectionRequestOne connectionRequestOne)
            {
                if (connectionRequestOne.Protocol != RakNet.SERVER_NETWORK_PROTOCOL)
                    throw new ProtocolViolationException($"protocol number {connectionRequestOne.Protocol} not found.");

                OpenConnectionReplyOne replyOne =
                    (OpenConnectionReplyOne) Socket.PacketIdentifier.GetPacketFormId(PacketIdentifier
                        .OPEN_CONNECTION_REPLY_1);
                replyOne.MtuSize = connectionRequestOne.MtuSize;
                replyOne.ServerGuid = Guid;
                replyOne.EndPoint = connectionRequestOne.EndPoint;
                Socket.SendPacketAsync(replyOne);
            }
            else if (packet is OpenConnectionRequestTwo connectionRequestTwo)
            {
                if (Socket.EndPoint.Port != connectionRequestTwo.ConnectionEndPoint.Port)
                    throw new InvalidOperationException("connection port is not match.");

                try
                {
                    Connect(packet.EndPoint, connectionRequestTwo.ClientGuid, connectionRequestTwo.MtuSize);
                }
                catch (InvalidOperationException)
                {
                    MinecraftPeer peer = GetPeer<MinecraftPeer>(packet.EndPoint);
                    peer.Disconnect();
                    return;
                }

                OpenConnectionReplyTwo replyTwo =
                    (OpenConnectionReplyTwo) Socket.PacketIdentifier.GetPacketFormId(PacketIdentifier
                        .OPEN_CONNECTION_REPLY_2);
                replyTwo.MtuSize = connectionRequestTwo.MtuSize;
                replyTwo.ServerGuid = Guid;
                replyTwo.EndPoint = connectionRequestTwo.EndPoint;
                Socket.SendPacketAsync(replyTwo);
            }
        }

        public override void Connect(IPEndPoint endPoint, long clientId, ushort mtuSize)
        {
            MinecraftPeer peer = new MinecraftPeer(endPoint, clientId, mtuSize);
            AddPeer(peer);
            peer.Connect(this);
        }

        private void RegisterMinecraftDefaults()
        {
            PacketIdentifier identifier = Socket.PacketIdentifier;
            identifier.Register(CUSTOM_PACKET_0, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_1, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_2, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_3, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_4, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_5, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_6, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_7, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_8, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_9, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_A, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_B, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_C, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_D, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_E, typeof(CustomPacket));
            identifier.Register(CUSTOM_PACKET_F, typeof(CustomPacket));

            identifier.Register(ACK, typeof(AckPacket));
            identifier.Register(NACK, typeof(NackPacket));

            identifier.Register(BATCH_PACKET, typeof(BatchPacket));

            identifier.CompileAll();
        }
    }
}