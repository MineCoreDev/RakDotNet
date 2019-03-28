using System;
using System.IO;
using NUnit.Framework;
using RakDotNet.IO;

namespace RakDotNet.Tests
{
    [TestFixture]
    public class BinaryStreamTest
    {
        [Test]
        public void WriteReadTest()
        {
            var stream = new BinaryStream();
            stream.WriteByte(123);
            stream.WriteSByte(123);
            stream.WriteShort(11345);
            stream.WriteUShort(11345);
            stream.WriteShort(11345, ByteOrder.Little);
            stream.WriteUShort(11345, ByteOrder.Little);
            stream.WriteInt(11564567);
            stream.WriteUInt(11564567);
            stream.WriteInt(11564567, ByteOrder.Little);
            stream.WriteUInt(11564567, ByteOrder.Little);
            stream.WriteLong(91234567890);
            stream.WriteULong(91234567890);
            stream.WriteLong(91234567890, ByteOrder.Little);
            stream.WriteULong(91234567890, ByteOrder.Little);
            stream.WriteFloat(4123.456f);
            stream.WriteDouble(8123.456789);
            stream.WriteFloat(4123.456f, ByteOrder.Little);
            stream.WriteDouble(8123.456789, ByteOrder.Little);
            stream.WriteTriad(0xffffff);
            stream.WriteTriad(0xffffff, ByteOrder.Little);
            stream.WriteStringUtf8("strstrほげほげ");
            stream.WriteStringUtf8("strstrほげほげ", ByteOrder.Little);
            stream.WriteTimeSpan(new TimeSpan(0, 0, 5, 40, 91));
            stream.WriteDateTime(new DateTime(2019, 3, 22, 5, 40, 30, 300));

            var read = new BinaryStream(stream.ToArray());
            Assert.True(read.ReadByte() == 123);
            Assert.True(read.ReadSByte() == 123);
            Assert.True(read.ReadShort() == 11345);
            Assert.True(read.ReadUShort() == 11345);
            Assert.True(read.ReadShort(ByteOrder.Little) == 11345);
            Assert.True(read.ReadUShort(ByteOrder.Little) == 11345);
            Assert.True(read.ReadInt() == 11564567);
            Assert.True(read.ReadUInt() == 11564567);
            Assert.True(read.ReadInt(ByteOrder.Little) == 11564567);
            Assert.True(read.ReadUInt(ByteOrder.Little) == 11564567);
            Assert.True(read.ReadLong() == 91234567890);
            Assert.True(read.ReadULong() == 91234567890);
            Assert.True(read.ReadLong(ByteOrder.Little) == 91234567890);
            Assert.True(read.ReadULong(ByteOrder.Little) == 91234567890);
            Assert.True(Math.Abs(read.ReadFloat() - 4123.456f) < 0.001f);
            Assert.True(Math.Abs(read.ReadDouble() - 8123.456789) < 0.001d);
            Assert.True(Math.Abs(read.ReadFloat(ByteOrder.Little) - 4123.456f) < 0.001f);
            Assert.True(Math.Abs(read.ReadDouble(ByteOrder.Little) - 8123.456789) < 0.001d);
            Assert.True(read.ReadTriad() == 0xffffff);
            Assert.True(read.ReadTriad(ByteOrder.Little) == 0xffffff);
            Assert.True(read.ReadStringUtf8() == "strstrほげほげ");
            Assert.True(read.ReadStringUtf8(ByteOrder.Little) == "strstrほげほげ");
            Assert.True(read.ReadTimeSpan() ==
                        new TimeSpan(0, 0, 5, 40, 91));
            Assert.True(read.ReadDateTime() ==
                        new DateTime(2019, 3, 22, 5, 40, 30, 300));
        }

        [Test]
        public void StreamingTest()
        {
            var stream = new BinaryStream(new byte[]
            {
                0x11, 0x01, 0xff
            });
            Assert.True(stream.ReadByte() == 0x11);
            stream.WriteByte(123);
            stream.Position -= 1;
            Assert.True(stream.ReadByte() == 123);
            stream.Reset();
            Assert.True(stream.ReadByte() == 0x11);
        }

        [Test]
        public void OutOfStream()
        {
            var stream = new BinaryStream(new byte[]
            {
                0x11, 0x01, 0xff
            });
            Assert.Catch<IndexOutOfRangeException>(() => { stream.ReadInt(); });
        }

        [Test]
        public void SetBuffer()
        {
            var stream = new BinaryStream();
            stream.SetBuffer(new byte[]
            {
                0x00,
                0xff,
                0x00
            });
            Assert.True(stream.ReadByte() == 0x00);
            Assert.True(stream.ReadByte() == 0xff);
            Assert.True(stream.ReadByte() == 0x00);
        }

        [Test]
        public void ClearAndReset()
        {
            var stream = new BinaryStream(new byte[]
            {
                0x11, 0x01, 0xff
            });
            stream.Clear();
            Assert.Throws<IndexOutOfRangeException>(() => { stream.ReadByte(); });
        }

        [Test]
        public void ReadAllByte()
        {
            var stream = new BinaryStream(new byte[]
            {
                0x11, 0x01, 0xff
            });
            Assert.True(stream.ReadByte() == 0x11);
            Assert.True(stream.ReadBytes().Length == 2);
        }

        [Test]
        public void WriteLenByte()
        {
            var stream = new BinaryStream();
            stream.WriteBytes(new byte[]
            {
                0x11, 0x01, 0xff
            }, 2);
            Assert.True(stream.Position == 2);
        }

        [Test]
        public void BigOrderTest()
        {
            var stream = new BinaryStream();
            stream.WriteUInt(0x12345678);
            byte[] buf = stream.ToArray();
            Assert.True(buf[0] == 0x12);
            Assert.True(buf[1] == 0x34);
            Assert.True(buf[2] == 0x56);
            Assert.True(buf[3] == 0x78);
        }

        [Test]
        public void LittleOrderTest()
        {
            var stream = new BinaryStream();
            stream.WriteUInt(0x12345678, ByteOrder.Little);
            byte[] buf = stream.ToArray();
            Assert.True(buf[3] == 0x12);
            Assert.True(buf[2] == 0x34);
            Assert.True(buf[1] == 0x56);
            Assert.True(buf[0] == 0x78);
        }
    }
}