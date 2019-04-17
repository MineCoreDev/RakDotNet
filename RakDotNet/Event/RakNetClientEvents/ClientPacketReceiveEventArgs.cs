using RakDotNet.Protocols.Packets;

namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientPacketReceiveEventArgs : RakNetClientEventArgs
    {
        public RakNetPacket ReceivePacket { get; }
        public ulong ReceiveBytes { get; }

        public ClientPacketReceiveEventArgs(RakNetSocket socket, RakNetPacket packet, ulong bytes) : base(socket)
        {
            ReceivePacket = packet;
            ReceiveBytes = bytes;
        }
    }
}