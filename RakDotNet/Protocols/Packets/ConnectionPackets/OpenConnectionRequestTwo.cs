using System.Net;

namespace RakDotNet.Protocols.Packets.ConnectionPackets
{
    public class OpenConnectionRequestTwo : RakNetPacket
    {
        public override byte PacketId => PacketIdentifier.OPEN_CONNECTION_REQUEST_2;

        public IPEndPoint ConnectionEndPoint { get; set; }
        public ushort MtuSize { get; set; }
        public long ClientGuid { get; set; }

        public override void EncodePayload()
        {
            WriteMagic();
            WriteIpEndPoint(ConnectionEndPoint);
            WriteUShort(MtuSize);
            WriteLong(ClientGuid);
        }

        public override void DecodePayload()
        {
            CheckMagic();
            ConnectionEndPoint = ReadIpEndPoint();
            MtuSize = ReadUShort();
            ClientGuid = ReadLong();
        }
    }
}