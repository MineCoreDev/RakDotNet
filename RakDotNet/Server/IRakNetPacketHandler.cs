using RakDotNet.Protocols.Packets;

namespace RakDotNet.Server
{
    public interface IRakNetPacketHandler
    {
        void HandleRakNetPacket(RakNetPacket packet);
    }
}