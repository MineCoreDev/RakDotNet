using System;
using System.Collections.Generic;
using System.IO;
using RakDotNet.IO;

namespace RakDotNet.Utils
{
    public class VarInt
    {
        private static uint EncodeZigZag32(int n)
        {
            // Note:  the right-shift must be arithmetic
            return (uint) ((n << 1) ^ (n >> 31));
        }

        private static int DecodeZigZag32(uint n)
        {
            return (int) (n >> 1) ^ -(int) (n & 1);
        }

        private static ulong EncodeZigZag64(long n)
        {
            return (ulong) ((n << 1) ^ (n >> 63));
        }

        private static long DecodeZigZag64(ulong n)
        {
            return (long) (n >> 1) ^ -(long) (n & 1);
        }

        private static uint ReadRawVarInt32(BinaryStream stream, int maxSize)
        {
            uint result = 0;
            int j = 0;
            int b0;

            do
            {
                b0 = stream.ReadByte();
                if (b0 < 0) throw new EndOfStreamException("Not enough bytes for VarInt");

                result |= (uint) (b0 & 0x7f) << j++ * 7;

                if (j > maxSize)
                {
                    throw new OverflowException("VarInt too big");
                }
            } while ((b0 & 0x80) == 0x80);

            return result;
        }

        private static ulong ReadRawVarInt64(BinaryStream stream, int maxSize)
        {
            ulong result = 0;
            int j = 0;
            int b0;

            do
            {
                b0 = stream.ReadByte();
                if (b0 < 0) throw new EndOfStreamException("Not enough bytes for VarInt");

                result |= (ulong) (b0 & 0x7f) << j++ * 7;

                if (j > maxSize)
                {
                    throw new OverflowException("VarInt too big");
                }
            } while ((b0 & 0x80) == 0x80);

            return result;
        }

        private static void WriteRawVarInt32(BinaryStream stream, uint value)
        {
            while ((value & -128) != 0)
            {
                stream.WriteByte((byte) ((value & 0x7F) | 0x80));
                value >>= 7;
            }

            stream.WriteByte((byte) value);
        }

        private static void WriteRawVarInt64(BinaryStream stream, ulong value)
        {
            while ((value & 0xFFFFFFFFFFFFFF80) != 0)
            {
                stream.WriteByte((byte) ((value & 0x7F) | 0x80));
                value >>= 7;
            }

            stream.WriteByte((byte) value);
        }

        public static int ReadVarInt(BinaryStream stream)
        {
            return (int) ReadRawVarInt32(stream, 5);
        }

        public static void WriteVarInt(BinaryStream stream, int value)
        {
            WriteRawVarInt32(stream, (uint) value);
        }

        public static int ReadSVarInt(BinaryStream stream)
        {
            return DecodeZigZag32(ReadRawVarInt32(stream, 5));
        }

        public static void WriteSVarInt(BinaryStream stream, int value)
        {
            WriteRawVarInt32(stream, EncodeZigZag32(value));
        }

        public static uint ReadUVarInt(BinaryStream stream)
        {
            return ReadRawVarInt32(stream, 5);
        }

        public static void WriteUVarInt(BinaryStream stream, uint value)
        {
            WriteRawVarInt32(stream, value);
        }

        public static long ReadVarLong(BinaryStream stream)
        {
            return (long) ReadRawVarInt64(stream, 10);
        }

        public static void WriteVarLong(BinaryStream stream, long value)
        {
            WriteRawVarInt64(stream, (ulong) value);
        }

        public static long ReadSVarLong(BinaryStream stream)
        {
            return DecodeZigZag64(ReadRawVarInt64(stream, 10));
        }

        public static void WriteSVarLong(BinaryStream stream, long value)
        {
            WriteRawVarInt64(stream, EncodeZigZag64(value));
        }

        public static ulong ReadUVarLong(BinaryStream stream)
        {
            return ReadRawVarInt64(stream, 10);
        }

        public static void WriteUVarLong(BinaryStream stream, ulong value)
        {
            WriteRawVarInt64(stream, value);
        }
    }
}