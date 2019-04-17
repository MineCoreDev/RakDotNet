using System;
using System.Net;
using RakDotNet.Minecraft;

namespace RakDotNet.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MinecraftServer server = new MinecraftServer(new IPEndPoint(IPAddress.Any, 19132));
            server.ServerListData = "MCPE;RakNetServer;340;1.10.1;0;5;RakDotNet;RakNetTestServer";
            server.Start();

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                    break;
            }
        }
    }
}