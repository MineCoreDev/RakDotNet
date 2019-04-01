using System;

namespace RakDotNet.Protocols
{
    public class PacketEncodeException : Exception
    {
        public PacketEncodeException(string msg) : base(msg)
        {
        }
    }
}