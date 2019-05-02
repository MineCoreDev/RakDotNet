using RakDotNet.Protocols.Packets;

namespace RakDotNet.Event.RakNetSocketEvents
{
    public class SocketPacketSendEventArgs : RakNetSocketEventArgs
    {
        public RakNetPacket SendPacket { get; }
        public ulong SendBytes { get; }

        public SocketPacketSendEventArgs(RakNetSocket socket, RakNetPacket packet, ulong bytes) : base(socket)
        {
            SendPacket = packet;
            SendBytes = bytes;
        }
    }
}