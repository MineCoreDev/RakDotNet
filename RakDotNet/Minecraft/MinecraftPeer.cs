using System;
using System.Collections.Concurrent;
using System.Net;
using BinaryIO;
using RakDotNet.Minecraft.Packets;
using RakDotNet.Minecraft.Packets.Acknowledge;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.ConnectionPackets;
using RakDotNet.Protocols.Packets.LoginPackets;
using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Protocols.Packets.PingPackets;
using RakDotNet.Server.Peer;
using RakDotNet.Utils;

namespace RakDotNet.Minecraft
{
    public class MinecraftPeer : RakNetPeer
    {
        public Action<BatchPacket> HandleBatchPacket { private get; set; } =
            packet => { };

        public MinecraftPeer(IPEndPoint endPoint, long clientId, ushort mtuSize) : base(endPoint, clientId, mtuSize)
        {
            LastPingTime = (long) TimeSpan.FromMilliseconds(Environment.TickCount).TotalMilliseconds;
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
            RakNetPacket pk = Server.Socket.PacketIdentifier.GetPacketFormId(buffer[0]);
            pk.SetBuffer(buffer);

            pk.DecodeHeader();
            pk.DecodePayload();

            if (pk is ConnectionRequest connectionRequest && State == RakNetPeerState.Connected)
            {
                ConnectionRequestAccepted accepted = new ConnectionRequestAccepted();
                accepted.PeerAddress = PeerEndPoint;
                accepted.ClientTimestamp = connectionRequest.Timestamp;
                accepted.ServerTimestamp = TimeSpan.FromMilliseconds(Environment.TickCount);
                accepted.EndPoint = PeerEndPoint;
                SendEncapsulatedPacket(accepted, Reliability.Unreliable, 0);

                State = RakNetPeerState.Handshaking;
            }
            else if (pk is NewIncomingConnection connection && State == RakNetPeerState.Handshaking)
            {
                if (connection.RemoteServerAddress.Port != Server.Socket.EndPoint.Port)
                {
                    Disconnect("port miss match.");
                    return;
                }

                State = RakNetPeerState.LoggedIn;
            }
            else if (pk is ConnectedPing)
            {
                ConnectedPong pong = new ConnectedPong();
                pong.Timestamp = TimeSpan.FromMilliseconds(Environment.TickCount);
                pong.EndPoint = PeerEndPoint;

                LastPingTime = (long) pong.Timestamp.TotalMilliseconds;

                SendEncapsulatedPacket(pong, Reliability.Unreliable, 0);
            }
            else if (pk is DisconnectionNotification)
            {
                Disconnect("client disconnect.");
            }
            else if (pk is BatchPacket batchPacket && State == RakNetPeerState.LoggedIn)
            {
                HandleBatchPacket(batchPacket);
            }
        }

        public void SendAck(uint sequenceId)
        {
            AckPacket packet = (AckPacket) Server.Socket.PacketIdentifier.GetPacketFormId(MinecraftServer.ACK);
            packet.Records.Add(new Record(sequenceId));
            packet.EndPoint = PeerEndPoint;

            SendPacket(packet);
        }

        public void SendNack(uint sequenceId)
        {
            NackPacket packet = (NackPacket) Server.Socket.PacketIdentifier.GetPacketFormId(MinecraftServer.NACK);
            packet.Records.Add(new Record(sequenceId));
            packet.EndPoint = PeerEndPoint;

            SendPacket(packet);
        }

        public void SendEncapsulatedPacket(RakNetPacket packet, Reliability reliability, byte flags)
        {
            packet.EncodeHeader();
            packet.EncodePayload();

            CustomPacket pk =
                Server.Socket.PacketIdentifier.GetPacketFormId(MinecraftServer.CUSTOM_PACKET_4) as CustomPacket;

            EncapsulatedPacket encapsulatedPacket = new EncapsulatedPacket();
            encapsulatedPacket.Reliability = reliability;
            encapsulatedPacket.Payload = packet.GetBuffer();

            pk.SequenceId = SendSequenceNumber++;
            pk.Packets = new[] {encapsulatedPacket};
            pk.EndPoint = packet.EndPoint;
            SendPacket(pk);
        }

        public override void Disconnect(string reason)
        {
            Logger.Info(reason);
            SendEncapsulatedPacket(new DisconnectionNotification
            {
                EndPoint = PeerEndPoint
            }, Reliability.Reliable, 0);
            Server.Disconnect(PeerEndPoint);
        }
    }
}