using RakDotNet.Protocols.Packets;

namespace RakDotNet.Event.RakNetSocketEvents
{
    public class SocketPacketReceiveEventArgs : RakNetSocketEventArgs
    {
        public RakNetPacket ReceivePacket { get; }
        public ulong ReceiveBytes { get; }

        public SocketPacketReceiveEventArgs(RakNetSocket socket, RakNetPacket packet, ulong bytes) : base(socket)
        {
            ReceivePacket = packet;
            ReceiveBytes = bytes;
        }
    }
}