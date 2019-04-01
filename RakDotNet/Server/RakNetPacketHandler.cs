using RakDotNet.Protocols.Packets;

namespace RakDotNet.Server
{
    public interface RakNetPacketHandler
    {
        void HandleRakNetPacket(RakNetPacket packet);
    }
}