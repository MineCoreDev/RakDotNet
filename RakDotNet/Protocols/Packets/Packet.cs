using System.Net;
using RakDotNet.IO;

namespace RakDotNet.Protocols.Packets
{
    public abstract class Packet : NetworkStream
    {
        public IPEndPoint EndPoint { get; set; }

        public virtual void EncodeHeader()
        {
        }

        public virtual void EncodePayload()
        {
        }

        public virtual void DecodeHeader()
        {
        }

        public virtual void DecodePayload()
        {
        }
    }
}