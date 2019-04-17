using RakDotNet.Protocols.Packets;

namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientPacketSendEventArgs : RakNetClientEventArgs
    {
        public RakNetPacket SendPacket { get; }
        public ulong SendBytes { get; }

        public ClientPacketSendEventArgs(RakNetSocket socket, RakNetPacket packet, ulong bytes) : base(socket)
        {
            SendPacket = packet;
            SendBytes = bytes;
        }
    }
}