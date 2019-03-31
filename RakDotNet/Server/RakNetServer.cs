using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Server.Peer;

namespace RakDotNet.Server
{
    public class RakNetServer
    {
        private ConcurrentDictionary<IPEndPoint, RakNetPeer> _connectedPeers =
            new ConcurrentDictionary<IPEndPoint, RakNetPeer>();

        public RakNetClient Client { get; }

        public RakNetServer(IPEndPoint endPoint)
        {
            Client = new RakNetClient(endPoint);
        }

        public RakNetServer(IPEndPoint endPoint, PacketIdentifier identifier)
        {
            Client = new RakNetClient(endPoint, identifier);
        }

        public void Start()
        {
            Client.StartReceiveWorker();
            Client.OnReceive = HandleRakNetPacket;
        }

        public T GetPeer<T>(IPEndPoint endPoint) where T : RakNetPeer
        {
            if (_connectedPeers.ContainsKey(endPoint))
                return (T) _connectedPeers[endPoint];
            else
                throw new KeyNotFoundException();
        }

        public virtual void Connect(IPEndPoint endPoint)
        {
            RakNetPeer peer = new RakNetPeer(endPoint);
            AddPeer(endPoint, peer);
            peer.Connect(this);
        }

        public void Disconnect(IPEndPoint endPoint)
        {
            if (_connectedPeers.ContainsKey(endPoint))
            {
                RemovePeer(endPoint);
            }
        }

        protected virtual void HandleRakNetPacket(RakNetPacket packet)
        {
        }

        protected void AddPeer<T>(IPEndPoint endPoint, T peer) where T : RakNetPeer
        {
            if (!_connectedPeers.ContainsKey(endPoint))
                _connectedPeers.TryAdd(endPoint, peer);
            else
                throw new InvalidOperationException();
        }

        protected void RemovePeer(IPEndPoint endPoint)
        {
            if (_connectedPeers.ContainsKey(endPoint))
                _connectedPeers.TryRemove(endPoint, out RakNetPeer peer);
            else
                throw new KeyNotFoundException();
        }
    }
}