using System.Net;
using BinaryIO;

namespace RakDotNet.Utils
{
    public struct RawPacket
    {
        public IPEndPoint EndPoint { get; }
        public NetworkStream Stream { get; }

        public RawPacket(IPEndPoint endPoint, NetworkStream stream)
        {
            EndPoint = endPoint;
            Stream = stream;
        }
    }
}