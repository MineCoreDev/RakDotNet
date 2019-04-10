using System;
using System.Collections.Concurrent;
using System.Net;
using RakDotNet.IO;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Minecraft.Packets.Acknowledge;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.LoginPackets;
using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Server.Peer;
using RakDotNet.Utils;

namespace RakDotNet.Minecraft
{
    public class MinecraftPeer : RakNetPeer
    {
        public Action<EncapsulatedPacket> HandleBatchPacket { private get; set; } =
            (packet) => { };

        public MinecraftPeer(IPEndPoint endPoint, long clientId, ushort mtuSize) : base(endPoint, clientId, mtuSize)
        {
        }

        public override void HandlePeerPacket(RakNetPacket packet)
        {
            if (packet is CustomPacket customPacket)
                HandleCustomHandle(customPacket);
        }

        public void HandleCustomHandle(CustomPacket packet)
        {
            SendAck(packet.SequenceId);

            uint handleSeq = packet.SequenceId + 1;
            uint diff = handleSeq - ReceiveSequenceNumber;
            if (ReceiveSequenceNumber < handleSeq)
            {
                if (diff != 1)
                    for (uint i = ReceiveSequenceNumber; i < ReceiveSequenceNumber + diff - 1; i++)
                        SendNack(i - 1);

                ReceiveSequenceNumber = handleSeq;

                for (int i = 0; i < packet.Packets.Length; i++)
                {
                    HandleEncapsulatedPacket(packet.Packets[i]);
                }
            }
        }

        public override void HandleEncapsulatedPacket(EncapsulatedPacket packet)
        {
            if (packet.Reliability.IsUnreliable())
            {
                HandleConnectedPacket(packet);
            }
            else if (packet.Reliability.IsReliable())
            {
                if (packet.MessageIndex < StartMessageWindow ||
                    packet.MessageIndex > EndMessageWindow || MessageWindow.ContainsKey(packet.MessageIndex))
                    return;

                MessageWindow.TryAdd(packet.MessageIndex, true);
                if (packet.MessageIndex == StartMessageWindow)
                {
                    for (; MessageWindow.ContainsKey(StartMessageWindow); ++StartMessageWindow)
                    {
                        MessageWindow.TryRemove(StartMessageWindow, out bool v);

                        ++EndMessageWindow;
                    }
                }

                if (packet.Split)
                {
                    HandleSplitEncapsulatedPacket(packet);
                    return;
                }

                HandleConnectedPacket(packet);
            }
        }

        public void HandleSplitEncapsulatedPacket(EncapsulatedPacket packet)
        {
            Logger.Log(packet.GetPacketSize());
            if (!SplitPackets.ContainsKey(packet.SplitId))
            {
                SplitPackets.TryAdd(packet.SplitId, new ConcurrentDictionary<int, EncapsulatedPacket>());
                if (!SplitPackets[packet.SplitId].ContainsKey(packet.SplitIndex))
                {
                    SplitPackets[packet.SplitId].TryAdd(packet.SplitIndex, packet);
                }
            }
            else
            {
                if (!SplitPackets[packet.SplitId].ContainsKey(packet.SplitIndex))
                {
                    SplitPackets[packet.SplitId].TryAdd(packet.SplitIndex, packet);
                }
            }

            if (SplitPackets[packet.SplitId].Count == packet.SplitCount)
            {
                EncapsulatedPacket pk = new EncapsulatedPacket();
                BinaryStream stream = new BinaryStream();
                for (int i = 0; i < packet.SplitCount; ++i)
                {
                    EncapsulatedPacket p = SplitPackets[packet.SplitId][i];
                    byte[] buffer = p.Payload;
                    stream.WriteBytes(buffer);
                }

                pk.Payload = stream.ToArray();

                SplitPackets.TryRemove(pk.SplitId, out ConcurrentDictionary<int, EncapsulatedPacket> d);

                HandleConnectedPacket(pk);
            }
        }

        public void HandleConnectedPacket(EncapsulatedPacket packet)
        {
            byte[] buffer = packet.Payload;
            RakNetPacket pk = Server.Client.PacketIdentifier.GetPacketFormId(buffer[0]);
            pk.SetBuffer(buffer);

            pk.DecodeHeader();
            pk.DecodePayload();

            if (pk is ConnectionRequest connectionRequest && State == RakNetPeerState.Connected)
            {
                ConnectionRequestAccepted accepted = new ConnectionRequestAccepted();
                accepted.PeerAddress = PeerEndPoint;
                accepted.ClientTimestamp = connectionRequest.Timestamp;
                accepted.ServerTimestamp = TimeSpan.FromTicks(Environment.TickCount);
                accepted.EndPoint = PeerEndPoint;
                SendEncapsulatedPacket(accepted, Reliability.Unreliable, 0);

                State = RakNetPeerState.Handshaking;
            }
            else if (pk is NewIncomingConnection connection && State == RakNetPeerState.Handshaking)
            {
                if (connection.RemoteServerAddress.Port != Server.Client.EndPoint.Port)
                {
                    Disconnect("port miss match.");
                    return;
                }

                State = RakNetPeerState.LoggedIn;
            }
            else if (State == RakNetPeerState.LoggedIn)
            {
                HandleBatchPacket(packet);
            }
        }

        public void SendAck(uint sequenceId)
        {
            AckPacket packet = (AckPacket) Server.Client.PacketIdentifier.GetPacketFormId(MinecraftServer.ACK);
            packet.Records.Add(new Record(sequenceId));
            packet.EndPoint = PeerEndPoint;

            SendPacket(packet);
        }

        public void SendNack(uint sequenceId)
        {
            NackPacket packet = (NackPacket) Server.Client.PacketIdentifier.GetPacketFormId(MinecraftServer.NACK);
            packet.Records.Add(new Record(sequenceId));
            packet.EndPoint = PeerEndPoint;

            SendPacket(packet);
        }

        public void SendEncapsulatedPacket(RakNetPacket packet, Reliability reliability, byte flags)
        {
            packet.EncodeHeader();
            packet.EncodePayload();

            CustomPacket pk =
                Server.Client.PacketIdentifier.GetPacketFormId(MinecraftServer.CUSTOM_PACKET_4) as CustomPacket;

            EncapsulatedPacket encapsulatedPacket = new EncapsulatedPacket();
            encapsulatedPacket.Reliability = reliability;
            encapsulatedPacket.Payload = packet.GetBuffer();

            pk.SequenceId = ReceiveSequenceNumber++;
            pk.Packets = new[] {encapsulatedPacket};
            pk.EndPoint = packet.EndPoint;
            Server.Client.SendPacket(pk);
        }
    }
}