using System;
using System.Net;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.Minecraft;
using RakDotNet.Utils;

namespace RakDotNet.Server.Minecraft
{
    public class MinecraftServer : RakNetServer
    {
        public String ServerListData { get; set; } = String.Empty;

        public MinecraftServer(IPEndPoint endPoint) : base(endPoint)
        {
            Client.PacketIdentifier.RegisterMinecraftDefaults();
        }

        public MinecraftServer(IPEndPoint endPoint, PacketIdentifier identifier) : base(endPoint, identifier)
        {
        }

        protected override void HandleRakNetPacket(RakNetPacket packet)
        {
            if (packet is UnconnectedPing unconnectedPing)
            {
                UnconnectedPong pong =
                    (UnconnectedPong) Client.PacketIdentifier.GetPacketFormId(PacketIdentifier.UNCONNECTED_PONG);
                pong.TimeStamp = TimeSpan.FromMilliseconds(Environment.TickCount);
                pong.PongId = unconnectedPing.PingId;
                pong.Identifier = ServerListData;
                pong.EndPoint = unconnectedPing.EndPoint;
                Client.SendPacket(pong);
            }
            else if (packet is OpenConnectionRequestOne connectionRequestOne)
            {
                
            }
        }
    }
}