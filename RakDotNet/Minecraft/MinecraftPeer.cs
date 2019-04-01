using System.Net;
using RakDotNet.Server.Peer;

namespace RakDotNet.Minecraft
{
    public class MinecraftPeer : RakNetPeer
    {
        public MinecraftPeer(IPEndPoint endPoint) : base(endPoint)
        {
        }
    }
}