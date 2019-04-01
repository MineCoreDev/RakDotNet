using System;

namespace RakDotNet.Protocols
{
    public class PacketDecodeException : Exception
    {
        public PacketDecodeException(string msg) : base(msg)
        {
        }
    }
}