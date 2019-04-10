using RakDotNet.Protocols.Packets;

namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientPacketReceiveEventArgs : RakNetClientEventArgs
    {
        public RakNetPacket ReceivePacket { get; }
        public ulong ReceiveBytes { get; }

        public ClientPacketReceiveEventArgs(RakNetClient client, RakNetPacket packet, ulong bytes) : base(client)
        {
            ReceivePacket = packet;
            ReceiveBytes = bytes;
        }
    }
}