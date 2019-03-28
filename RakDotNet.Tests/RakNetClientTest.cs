using System;
using System.ComponentModel;
using System.Net;
using NUnit.Framework;
using RakDotNet.IO;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;

namespace RakDotNet.Tests
{
    [TestFixture]
    public class RakNetClientTest
    {
        [Test]
        public void StartRakNetClient()
        {
            RakNetClient client = RakNet.CreateClient(new IPEndPoint(IPAddress.Any, 19132));
            //client.OnReceive = packet => { Console.WriteLine(packet.ReadByte()); };
            RakNetPacket p = client.ReceivePacket().GetAwaiter().GetResult();
            if (p is UnconnectedPing unconnectedPing)
            {
                Console.WriteLine(p.PacketId);
                Console.WriteLine(unconnectedPing.TimeStamp);
                Console.WriteLine(unconnectedPing.PingId);
            }
        }
    }
}