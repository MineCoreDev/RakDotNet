using RakDotNet.Protocols.Packets;

namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientPacketSendEventArgs : RakNetClientEventArgs
    {
        public RakNetPacket SendPacket { get; }
        public ulong SendBytes { get; }

        public ClientPacketSendEventArgs(RakNetClient client, RakNetPacket packet, ulong bytes) : base(client)
        {
            SendPacket = packet;
            SendBytes = bytes;
        }
    }
}