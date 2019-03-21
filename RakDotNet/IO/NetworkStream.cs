using RakDotNet.Utils;

namespace RakDotNet.IO
{
    public class NetworkStream : BinaryStream
    {
        public NetworkStream()
        {
        }

        public NetworkStream(byte[] buffer) : base(buffer)
        {
        }

        public int ReadVarInt()
        {
            return VarInt.ReadVarInt(this);
        }

        public void WriteVarInt(int value)
        {
            VarInt.WriteVarInt(this, value);
        }

        public int ReadSVarInt()
        {
            return VarInt.ReadSVarInt(this);
        }

        public void WriteSVarInt(int value)
        {
            VarInt.WriteSVarInt(this, value);
        }

        public uint ReadUVarInt()
        {
            return VarInt.ReadUVarInt(this);
        }

        public void WriteUVarInt(uint value)
        {
            VarInt.WriteUVarInt(this, value);
        }

        public long ReadVarLong()
        {
            return VarInt.ReadVarLong(this);
        }

        public void WriteVarLong(long value)
        {
            VarInt.WriteVarLong(this, value);
        }

        public long ReadSVarLong()
        {
            return VarInt.ReadSVarLong(this);
        }

        public void WriteSVarLong(long value)
        {
            VarInt.WriteSVarLong(this, value);
        }

        public ulong ReadUVarLong()
        {
            return VarInt.ReadUVarLong(this);
        }

        public void WriteUVarLong(ulong value)
        {
            VarInt.WriteUVarLong(this, value);
        }
    }
}