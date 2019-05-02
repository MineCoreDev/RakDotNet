using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BinaryIO;
using RakDotNet.Event;
using RakDotNet.Event.RakNetServerEvents;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Server.Peer;
using RakDotNet.Utils;

namespace RakDotNet.Server
{
    public class RakNetServer : IRakNetPacketHandler, IDisposable
    {
        private ConcurrentDictionary<IPEndPoint, RakNetPeer> _connectedPeers =
            new ConcurrentDictionary<IPEndPoint, RakNetPeer>();

        public RakNetSocket Socket { get; }

        public long Guid { get; private set; }
        public long PongId { get; private set; }

        public Task ServerWorker { get; private set; }
        public CancellationTokenSource WorkerCancelToken { get; private set; }

        public event EventHandler<ServerStartWorkerEventArgs> StartWorkerEvent;
        public event EventHandler<ServerStopWorkerEventArgs> StopWorkerEvent;
        public event EventHandler<ServerConnectPeerEventArgs> ConnectPeerEvent;
        public event EventHandler<ServerDisconnectPeerEventArgs> DisconnectPeerEvent;
        public event EventHandler<ServerPacketHandleEventArgs> PacketHandleEvent;

        public RakNetServer(IPEndPoint endPoint)
        {
            Socket = new RakNetSocket(endPoint);
            Init();
        }

        public RakNetServer(IPEndPoint endPoint, PacketIdentifier identifier)
        {
            Socket = new RakNetSocket(endPoint, identifier);
            Init();
        }

        public void Start()
        {
            StartServerWorker();
            Socket.StartReceiveWorker();
            Socket.OnReceive = HandleRakNetPacket;
        }

        public void StartServerWorker()
        {
            new ServerStartWorkerEventArgs(this)
                .Invoke(this, StartWorkerEvent);

            WorkerCancelToken = new CancellationTokenSource();
            ServerWorker = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (WorkerCancelToken.IsCancellationRequested)
                    {
                        new ServerStopWorkerEventArgs(this)
                            .Invoke(this, StopWorkerEvent);

                        Logger.Info($"{ServerWorker.Id} is Canceled.");
                        WorkerCancelToken.Token.ThrowIfCancellationRequested();
                    }

                    foreach (RakNetPeer peer in _connectedPeers.Values)
                    {
                        peer.Update();
                    }

                    await Task.Delay(1);
                }
            }, WorkerCancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public T GetPeer<T>(IPEndPoint endPoint) where T : RakNetPeer
        {
            if (_connectedPeers.ContainsKey(endPoint))
                return (T) _connectedPeers[endPoint];
            else
                throw new KeyNotFoundException();
        }

        public virtual void Connect(IPEndPoint endPoint, long clientId, ushort mtuSize)
        {
            RakNetPeer peer = new RakNetPeer(endPoint, clientId, mtuSize);
            AddPeer(peer);

            new ServerConnectPeerEventArgs(this, peer)
                .Invoke(this, ConnectPeerEvent);

            peer.Connect(this);
        }

        public void Disconnect(IPEndPoint endPoint)
        {
            if (_connectedPeers.TryGetValue(endPoint, out RakNetPeer peer))
            {
                new ServerDisconnectPeerEventArgs(this, peer)
                    .Invoke(this, DisconnectPeerEvent);

                RemovePeer(endPoint);
            }
        }

        public bool IsConnected(IPEndPoint endPoint)
        {
            return _connectedPeers.ContainsKey(endPoint);
        }

        public virtual void HandleRakNetPacket(RakNetPacket packet)
        {
            new ServerPacketHandleEventArgs(this, packet)
                .Invoke(this, PacketHandleEvent);
        }

        protected void AddPeer<T>(T peer) where T : RakNetPeer
        {
            if (!_connectedPeers.ContainsKey(peer.PeerEndPoint))
                _connectedPeers.TryAdd(peer.PeerEndPoint, peer);
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

        private void Init()
        {
            Guid guid = System.Guid.NewGuid();
            BinaryStream bs = new BinaryStream(guid.ToByteArray());
            Guid = bs.ReadLong();
            PongId = bs.ReadLong();
            bs.Close();
        }

        public virtual void Dispose()
        {
            WorkerCancelToken.Cancel();
            WorkerCancelToken = null;
            ServerWorker?.Dispose();
            Socket?.Dispose();
        }
    }
}