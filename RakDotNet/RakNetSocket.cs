using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RakDotNet.Event;
using RakDotNet.Event.RakNetSocketEvents;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Utils;
using NetworkStream = BinaryIO.NetworkStream;

namespace RakDotNet
{
    public class RakNetSocket : UdpClient
    {
        public PacketIdentifier PacketIdentifier { get; }

        public IPEndPoint EndPoint { get; }

        public ulong DownloadBytes { get; private set; }
        public ulong UploadBytes { get; private set; }

        public Task NetworkWorker { get; private set; }
        public CancellationTokenSource WorkerCancelToken { get; private set; }

        public Action<RakNetPacket> OnReceive { private get; set; }

        public event EventHandler<SocketStartWorkerEventArgs> StartWorkerEvent;
        public event EventHandler<SocketStopWorkerEventArgs> StopWorkerEvent;
        public event EventHandler<SocketPacketSendEventArgs> SendPacketEvent;
        public event EventHandler<SocketPacketReceiveEventArgs> ReceivePacketEvent;

        public RakNetSocket(IPEndPoint endPoint) : base(endPoint)
        {
            EndPoint = endPoint;
            PacketIdentifier = new PacketIdentifier();
        }

        public RakNetSocket(IPEndPoint endPoint, PacketIdentifier identifier) : base(endPoint)
        {
            EndPoint = endPoint;
            PacketIdentifier = identifier;
        }

        public void StartReceiveWorker()
        {
            new SocketStartWorkerEventArgs(this)
                .Invoke(this, StartWorkerEvent);

            WorkerCancelToken = new CancellationTokenSource();
            NetworkWorker = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (WorkerCancelToken.IsCancellationRequested)
                    {
                        new SocketStopWorkerEventArgs(this)
                            .Invoke(this, StopWorkerEvent);

                        Logger.Info($"{NetworkWorker.Id} is Canceled.");
                        WorkerCancelToken.Token.ThrowIfCancellationRequested();
                    }

                    try
                    {
                        OnReceive?.Invoke(await ReceivePacketAsync());
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e);
                    }

                    await Task.Delay(1);
                }
            }, WorkerCancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public RakNetPacket ReceivePacket()
        {
            IPEndPoint endPoint = null;
            byte[] buffer = Receive(ref endPoint);
            RakNetPacket packet = PacketIdentifier.GetPacketFormId(buffer[0]);
            packet.SetBuffer(buffer);
            packet.EndPoint = endPoint;
            DownloadBytes += (uint) buffer.Length;

            try
            {
                packet.DecodeHeader();
                packet.DecodePayload();
            }
            catch (Exception e)
            {
                throw new PacketDecodeException(e.Message, e);
            }

            new SocketPacketReceiveEventArgs(this, packet, DownloadBytes)
                .Invoke(this, ReceivePacketEvent);

            return packet;
        }

        public void SendPacket(RakNetPacket packet)
        {
            try
            {
                packet.EncodeHeader();
                packet.EncodePayload();
            }
            catch (Exception e)
            {
                throw new PacketEncodeException(e.Message, e);
            }

            byte[] buf = packet.GetBuffer();
            UploadBytes += (ulong) Send(buf, buf.Length, packet.EndPoint);

            new SocketPacketSendEventArgs(this, packet, UploadBytes)
                .Invoke(this, SendPacketEvent);

            packet.Close();
        }

        public async Task<RakNetPacket> ReceivePacketAsync()
        {
            UdpReceiveResult result = await ReceiveAsync();
            byte[] buffer = result.Buffer;
            RakNetPacket packet = PacketIdentifier.GetPacketFormId(buffer[0]);
            packet.SetBuffer(buffer);
            packet.EndPoint = result.RemoteEndPoint;
            DownloadBytes += (uint) result.Buffer.Length;

            try
            {
                packet.DecodeHeader();
                packet.DecodePayload();
            }
            catch (Exception e)
            {
                throw new PacketDecodeException(e.Message, e);
            }

            new SocketPacketReceiveEventArgs(this, packet, DownloadBytes)
                .Invoke(this, ReceivePacketEvent);

            return packet;
        }

        public async void SendPacketAsync(RakNetPacket packet)
        {
            try
            {
                packet.EncodeHeader();
                packet.EncodePayload();
            }
            catch (Exception e)
            {
                throw new PacketEncodeException(e.Message, e);
            }

            byte[] buf = packet.GetBuffer();
            UploadBytes += (ulong) await SendAsync(buf, buf.Length, packet.EndPoint);

            new SocketPacketSendEventArgs(this, packet, UploadBytes)
                .Invoke(this, SendPacketEvent);

            packet.Close();
        }

        public async Task<RawPacket> ReceiveRawPacketAsync()
        {
            UdpReceiveResult result = await ReceiveAsync();
            byte[] buffer = result.Buffer;
            NetworkStream stream = new NetworkStream(buffer);
            DownloadBytes += (uint) buffer.Length;
            return new RawPacket(result.RemoteEndPoint, stream);
        }

        public async void SendRawPacketAsync(RawPacket packet)
        {
            NetworkStream stream = packet.Stream;
            UploadBytes += (ulong) await SendAsync(stream.GetBuffer(), (int) stream.Length, packet.EndPoint);
            stream.Close();
        }

        public new void Close()
        {
            WorkerCancelToken.Cancel();
            NetworkWorker.Wait();
            NetworkWorker.Dispose();
            WorkerCancelToken.Dispose();
            base.Close();
        }
    }
}