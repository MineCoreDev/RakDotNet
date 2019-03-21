using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RakDotNet.Protocols
{
    public class RakNetPacket : Packet
    {
        public readonly ReadOnlyCollection<byte> Magic =
            new ReadOnlyCollection<byte>(new List<byte>(new byte[16]
            {
                0x00, 0xff, 0xff, 0x00,
                0xfe, 0xfe, 0xfe, 0xfe,
                0xfd, 0xfd, 0xfd, 0xfd,
                0x12, 0x34, 0x56, 0x78
            }));

        public byte[] ReadMagic()
        {
            return ReadBytes(16);
        }

        public void WriteMagic()
        {
            WriteBytes(Magic.ToArray());
        }

        public bool IsMatchMagic()
        {
            byte[] magic = ReadMagic();
            for (int i = 0; i < 16; i++)
            {
                if (magic[i] != Magic[i])
                    return false;
            }

            return true;
        }

        public override void EncodeHeader()
        {
            WriteMagic();
        }

        public override void DecodeHeader()
        {
            if (!IsMatchMagic())
                throw new FormatException("Not match RakNet magic");
        }
    }
}