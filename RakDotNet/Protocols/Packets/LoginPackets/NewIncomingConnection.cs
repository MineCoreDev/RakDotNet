using System;
using System.Net;

namespace RakDotNet.Protocols.Packets.LoginPackets
{
    public class NewIncomingConnection : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.NEW_INCOMING_CONNECTION;

        public IPEndPoint RemoteServerAddress { get; set; }

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
            WriteIpEndPoint(RemoteServerAddress);
            for (int i = 0; i < 10; i++)
            {
                WriteIpEndPoint(RouteingAddress[i]);
            }

            WriteLong((long) ClientTimestamp.TotalMilliseconds);
            WriteLong((long) ServerTimestamp.TotalMilliseconds);
        }

        public override void DecodePayload()
        {
            RemoteServerAddress = ReadIpEndPoint();

            RouteingAddress = new IPEndPoint[20];
            for (int i = 0; i < 20; i++)
            {
                RouteingAddress[i] = ReadIpEndPoint();
            }

            ClientTimestamp = TimeSpan.FromMilliseconds(ReadLong());
            ServerTimestamp = TimeSpan.FromMilliseconds(ReadLong());
        }
    }
}