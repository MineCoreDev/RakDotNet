using System;
using System.IO;
using System.Linq;
using System.Text;

namespace RakDotNet.IO
{
    public class BinaryStream : MemoryStream
    {
        public BinaryStream()
        {
        }

        public BinaryStream(byte[] buffer) : base(buffer)
        {
        }

        public new byte ReadByte()
        {
            return (byte) base.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return (sbyte) ReadByte();
        }

        public void WriteSByte(sbyte value)
        {
            WriteByte((byte) value);
        }

        public short ReadShort(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToInt16(Reverse(ReadBytes(2), order));
        }

        public void WriteShort(short value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public ushort ReadUShort(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToUInt16(Reverse(ReadBytes(2), order));
        }

        public void WriteUShort(ushort value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public int ReadInt(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToInt32(Reverse(ReadBytes(4), order));
        }

        public void WriteInt(int value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public uint ReadUInt(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToUInt32(Reverse(ReadBytes(4), order));
        }

        public void WriteUInt(uint value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public long ReadLong(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToInt64(Reverse(ReadBytes(8), order));
        }

        public void WriteLong(long value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public ulong ReadULong(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToUInt64(Reverse(ReadBytes(8), order));
        }

        public void WriteULong(ulong value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public float ReadFloat(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToSingle(Reverse(ReadBytes(4), order));
        }

        public void WriteFloat(float value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public double ReadDouble(ByteOrder order = ByteOrder.Big)
        {
            return BitConverter.ToDouble(Reverse(ReadBytes(8), order));
        }

        public void WriteDouble(double value, ByteOrder order = ByteOrder.Big)
        {
            WriteBytes(Reverse(BitConverter.GetBytes(value), order));
        }

        public string ReadStringUtf8(ByteOrder order = ByteOrder.Big)
        {
            ushort len = ReadUShort(order);
            if (len > 0)
                return Encoding.UTF8.GetString(ReadBytes(len));
            else
                return string.Empty;
        }

        public void WriteStringUtf8(string value, ByteOrder order = ByteOrder.Big)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            int len = buffer.Length;
            if (len > ushort.MaxValue)
                throw new IOException($"Max String Length Range 0 ~ ${ushort.MaxValue}.");
            WriteUShort((ushort) len, order);
            WriteBytes(buffer);
        }

        public TimeSpan ReadTimeSpan(ByteOrder order = ByteOrder.Big)
        {
            return new TimeSpan(ReadLong(order));
        }

        public void WriteTimeSpan(TimeSpan span, ByteOrder order = ByteOrder.Big)
        {
            WriteLong(span.Ticks, order);
        }

        public DateTime ReadDateTime(ByteOrder order = ByteOrder.Big)
        {
            return DateTime.FromBinary(ReadLong(order));
        }

        public void WriteDateTime(DateTime time, ByteOrder order = ByteOrder.Big)
        {
            WriteLong(time.ToBinary(), order);
        }

        public byte[] ReadBytes(int length)
        {
            byte[] buff = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buff[i] = ReadByte();
            }

            return buff;
        }

        public void WriteBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                WriteByte(buffer[i]);
            }
        }

        private byte[] Reverse(byte[] buffer, ByteOrder order)
        {
            if (BitConverter.IsLittleEndian ^ order == ByteOrder.Little)
                return buffer.Reverse().ToArray();
            else
                return buffer;
        }
    }
}