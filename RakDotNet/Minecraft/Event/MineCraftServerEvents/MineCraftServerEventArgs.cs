using System;

namespace RakDotNet.Minecraft.Event.MineCraftServerEvents
{
    public class MineCraftServerEventArgs : EventArgs
    {
        public MinecraftServer Server { get; }

        public MineCraftServerEventArgs(MinecraftServer server)
        {
            Server = server;
        }
    }
}