using System;
using System.IO;
using System.Linq;
using System.Net;
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

        public bool ReadBoolean()
        {
            return ReadByte() > 0;
        }

        public void WriteBoolean(bool value)
        {
            WriteByte(value ? (byte) 1 : (byte) 0);
        }

        public new byte ReadByte()
        {
            int val = base.ReadByte();
            if (val != -1)
                return (byte) val;

            throw new IndexOutOfRangeException();
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

        public uint ReadTriad(ByteOrder order = ByteOrder.Big)
        {
            byte[] buffer = Reverse(ReadBytes(3), order);
            uint value = BitConverter.ToUInt32(new byte[]
            {
                buffer[0],
                buffer[1],
                buffer[2],
                0x00
            });

            return value;
        }

        public void WriteTriad(uint value, ByteOrder order = ByteOrder.Big)
        {
            byte[] buffer = Reverse(BitConverter.GetBytes(value & 0xffffff), order);
            WriteBytes(order == ByteOrder.Big
                ? new byte[] {buffer[1], buffer[2], buffer[3]}
                : new byte[] {buffer[0], buffer[1], buffer[2]});
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

        public Guid ReadGuid(ByteOrder order = ByteOrder.Big)
        {
            byte[] most = Reverse(ReadBytes(8), order);
            byte[] least = Reverse(ReadBytes(8), order);

            return new Guid(most.Concat(least).ToArray());
        }

        public void WriteGuid(Guid guid, ByteOrder order = ByteOrder.Big)
        {
            byte[] buffer = guid.ToByteArray();
            byte[] most = Reverse(buffer.Take(8).ToArray(), order);
            byte[] least = Reverse(buffer.Skip(8).ToArray(), order);

            WriteBytes(most);
            WriteBytes(least);
        }

        public IPEndPoint ReadIpEndPoint(ByteOrder order = ByteOrder.Big)
        {
            byte version = ReadByte();
            byte[] address;
            if (version == 4)
                address = new byte[4];
            else if (version == 6)
                address = new byte[16];
            else
                throw new NotSupportedException($"IPv{version} not support.");

            for (int i = 0; i < address.Length; i++)
            {
                address[i] = (byte) (~ReadByte() & 0xff);
            }

            if (version == 6)
                ReadBytes(10);

            ushort port = ReadUShort(order);
            return new IPEndPoint(new IPAddress(address), port);
        }

        public void WriteIpEndPoint(IPEndPoint endPoint, ByteOrder order = ByteOrder.Big)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            else if (endPoint.Address == null)
                throw new ArgumentNullException(nameof(endPoint.Address));

            byte[] address = endPoint.Address.GetAddressBytes();
            if (address.Length == 4)
                WriteByte(4);
            else if (address.Length == 16)
                WriteByte(6);
            else
                throw new NotSupportedException($"{address.Length} is not support length");

            for (int i = 0; i < address.Length; i++)
            {
                WriteByte((byte) (~address[i] & 0xff));
            }

            if (address.Length == 16)
                WriteBytes(new byte[10]);

            WriteUShort((ushort) endPoint.Port, order);
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

        public byte[] ReadBytes()
        {
            byte[] buff = new byte[Length - Position];
            int idx = 0;
            while (Length != Position)
            {
                buff[idx] = ReadByte();
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

        public void WriteBytes(byte[] buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                WriteByte(buffer[i]);
            }
        }

        public void Reset()
        {
            Position = 0;
        }

        public void Clear()
        {
            SetLength(0);
            Reset();
        }

        public bool IsEndOfStream()
        {
            return Length >= Position;
        }

        public void SetBuffer(byte[] buffer)
        {
            Clear();
            WriteBytes(buffer);
            Reset();
        }

        public override byte[] GetBuffer()
        {
            return ToArray();
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