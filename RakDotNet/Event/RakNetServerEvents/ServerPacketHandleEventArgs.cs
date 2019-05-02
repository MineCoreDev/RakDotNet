using RakDotNet.Protocols.Packets;
using RakDotNet.Server;

namespace RakDotNet.Event.RakNetServerEvents
{
    public class ServerPacketHandleEventArgs : RakNetServerEventArgs
    {
        public RakNetPacket Packet { get; }

        public ServerPacketHandleEventArgs(RakNetServer server, RakNetPacket packet) : base(server)
        {
            Packet = packet;
        }
    }
}