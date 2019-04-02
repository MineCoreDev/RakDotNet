using System.Net;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Server.Peer;

namespace RakDotNet.Minecraft
{
    public class MinecraftPeer : RakNetPeer
    {
        public MinecraftPeer(IPEndPoint endPoint, long clientId, ushort mtuSize) : base(endPoint, clientId, mtuSize)
        {
        }
    }
}