using System.Net;
using RakDotNet.IO;

namespace RakDotNet.Protocols
{
    public class Packet : NetworkStream
    {
        public IPEndPoint EndPoint { get; set; }

        public Packet()
        {
        }

        public Packet(byte[] buffer) : base(buffer)
        {
        }

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