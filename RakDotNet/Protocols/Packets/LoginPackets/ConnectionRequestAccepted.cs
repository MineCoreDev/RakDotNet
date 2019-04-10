using System;
using System.Net;

namespace RakDotNet.Protocols.Packets.LoginPackets
{
    public class ConnectionRequestAccepted : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.CONNECTION_REQUEST_ACCEPTED;

        public IPEndPoint PeerAddress { get; set; }

        public IPEndPoint[] RouteingAddress { get; set; } = new IPEndPoint[10]
        {
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0),
            new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0)
        };

        public TimeSpan ClientTimestamp { get; set; }
        public TimeSpan ServerTimestamp { get; set; }

        public override void EncodePayload()
        {
            WriteIpEndPoint(PeerAddress);
            WriteShort(0);
            for (int i = 0; i < 10; i++)
            {
                WriteIpEndPoint(RouteingAddress[i]);
            }

            WriteLong((long) ClientTimestamp.TotalMilliseconds);
            WriteLong((long) ServerTimestamp.TotalMilliseconds);
        }

        public override void DecodePayload()
        {
            PeerAddress = ReadIpEndPoint();
            ReadShort();

            RouteingAddress = new IPEndPoint[10];
            for (int i = 0; i < 10; i++)
            {
                RouteingAddress[i] = ReadIpEndPoint();
            }

            ClientTimestamp = TimeSpan.FromMilliseconds(ReadLong());
            ServerTimestamp = TimeSpan.FromMilliseconds(ReadLong());
        }
    }
}