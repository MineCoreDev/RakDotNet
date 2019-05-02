namespace RakDotNet.Minecraft.Event.MineCraftServerEvents
{
    public class MineCraftServerConnectPeerEventArgs : MineCraftServerEventArgs
    {
        public MinecraftPeer Peer { get; }
        
        public MineCraftServerConnectPeerEventArgs(MinecraftServer server, MinecraftPeer peer) : base(server)
        {
            Peer = peer;
        }
    }
}