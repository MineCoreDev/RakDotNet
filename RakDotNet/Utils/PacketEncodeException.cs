using System;

namespace RakDotNet.Utils
{
    public class PacketEncodeException : Exception
    {
        public PacketEncodeException(string msg, Exception e) : base(msg, e)
        {
        }
    }
}