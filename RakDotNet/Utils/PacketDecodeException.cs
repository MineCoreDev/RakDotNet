using System;

namespace RakDotNet.Utils
{
    public class PacketDecodeException : Exception
    {
        public PacketDecodeException(string msg, Exception e) : base(msg, e)
        {
        }
    }
}