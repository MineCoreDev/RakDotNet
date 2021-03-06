using System;
using System.Collections.Concurrent;
using System.Net;
using RakDotNet.Event;
using RakDotNet.Event.RakNetPeerEvents;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.MessagePackets;
using RakDotNet.Utils;

namespace RakDotNet.Server.Peer
{
    public class RakNetPeer : IRakNetPeerPacketHandler
    {
        public IPEndPoint PeerEndPoint { get; }
        public RakNetServer Server { get; private set; }

        public long ClientId { get; }
        public ushort MtuSize { get; }
        public RakNetPeerState State { get; protected set; } = RakNetPeerState.Connected;
        public long TimeOut { get; set; } = 5000;

        public long LastPingTime { get; set; } = 0;

        public uint SendSequenceNumber { get; protected set; }
        public uint ReceiveSequenceNumber { get; protected set; }

        public uint StartMessageWindow { get; protected set; }
        public uint EndMessageWindow { get; protected set; } = 2048;

        public uint SendMessageIndex { get; protected set; }

        public ConcurrentDictionary<byte, uint> OrderIndexs = new ConcurrentDictionary<byte, uint>();

        public ConcurrentDictionary<uint, bool> MessageWindow { get; } = new ConcurrentDictionary<uint, bool>();

        public ConcurrentDictionary<ushort, ConcurrentDictionary<int, EncapsulatedPacket>> SplitPackets =
            new ConcurrentDictionary<ushort, ConcurrentDictionary<int, EncapsulatedPacket>>();

        public ushort SplitID { get; protected set; }

        public event EventHandler<PeerHandlePacketEventArgs> PeerHandlePacketEvent;
        public event EventHandler<PeerHandleEncapsulatesPacketEventArgs> PeerHandleEncapsulatesPacketEvent;
        public event EventHandler<PeerTimedOutEventArgs> PeerTimedOutEvent;

        public RakNetPeer(IPEndPoint endPoint, long clientId, ushort mtuSize)
        {
            PeerEndPoint = endPoint;
            ClientId = clientId;
            MtuSize = mtuSize;
        }

        public virtual void Connect(RakNetServer server)
        {
            Server = server;
        }

        public virtual void Disconnect()
        {
            Server.Disconnect(PeerEndPoint);
        }

        public virtual void Disconnect(string reason)
        {
            Logger.Info(reason);
            Server.Disconnect(PeerEndPoint);
        }

        public virtual void HandlePeerPacket(RakNetPacket packet)
        {
            new PeerHandlePacketEventArgs(this, packet)
                .Invoke(this, PeerHandlePacketEvent);
        }

        public virtual void HandleEncapsulatedPacket(EncapsulatedPacket packet)
        {
            new PeerHandleEncapsulatesPacketEventArgs(this, packet)
                .Invoke(this, PeerHandleEncapsulatesPacketEvent);
        }

        public void Update()
        {
            TimeSpan span = TimeSpan.FromMilliseconds(Environment.TickCount);
            if (span.TotalMilliseconds - LastPingTime > TimeOut)
            {
                new PeerTimedOutEventArgs(this)
                    .Invoke(this, PeerTimedOutEvent);

                Disconnect("timed out.");
            }
        }

        public void SendPacket(RakNetPacket packet)
        {
            Server.Socket.SendPacket(packet);
        }
    }
}