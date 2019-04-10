using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RakDotNet.Event;
using RakDotNet.Event.RakNetClientEvents;
using RakDotNet.IO;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Utils;
using NetworkStream = RakDotNet.IO.NetworkStream;

namespace RakDotNet
{
    public class RakNetClient : UdpClient
    {
        public PacketIdentifier PacketIdentifier { get; }

        public IPEndPoint EndPoint { get; }

        public ulong DownloadBytes { get; private set; }
        public ulong UploadBytes { get; private set; }

        public Task NetworkWorker { get; private set; }
        public CancellationTokenSource WorkerCancelToken { get; private set; }

        public Action<RakNetPacket> OnReceive { private get; set; }

        public event EventHandler<ClientStartWorkerEventArgs> StartWorkerEvent;
        public event EventHandler<ClientStopWorkerEventArgs> StopWorkerEvent;
        public event EventHandler<ClientPacketSendEventArgs> SendPacketEvent;
        public event EventHandler<ClientPacketReceiveEventArgs> ReceivePacketEvent;

        public RakNetClient(IPEndPoint endPoint) : base(endPoint)
        {
            EndPoint = endPoint;
            PacketIdentifier = new PacketIdentifier();
        }

        public RakNetClient(IPEndPoint endPoint, PacketIdentifier identifier) : base(endPoint)
        {
            EndPoint = endPoint;
            PacketIdentifier = identifier;
        }

        public void StartReceiveWorker()
        {
            new ClientStartWorkerEventArgs(this)
                .Invoke(this, StartWorkerEvent);

            WorkerCancelToken = new CancellationTokenSource();
            NetworkWorker = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (WorkerCancelToken.IsCancellationRequested)
                    {
                        new ClientStopWorkerEventArgs(this)
                            .Invoke(this, StopWorkerEvent);

                        Logger.Debug($"{NetworkWorker.Id} is Canceled.");
                        WorkerCancelToken.Token.ThrowIfCancellationRequested();
                    }

                    try
                    {
                        OnReceive?.Invoke(await ReceivePacket());
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e);
                    }
                }
            }, WorkerCancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task<RakNetPacket> ReceivePacket()
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
                Logger.Log(packet.Length);
                throw new PacketDecodeException(e.Message);
            }

            new ClientPacketReceiveEventArgs(this, packet, DownloadBytes)
                .Invoke(this, ReceivePacketEvent);

            return packet;
        }

        public async Task<RawPacket> ReceiveRawPacket()
        {
            UdpReceiveResult result = await ReceiveAsync();
            byte[] buffer = result.Buffer;
            NetworkStream stream = new NetworkStream(buffer);
            DownloadBytes += (uint) buffer.Length;
            return new RawPacket(result.RemoteEndPoint, stream);
        }

        public async void SendPacket(RakNetPacket packet)
        {
            try
            {
                packet.EncodeHeader();
                packet.EncodePayload();
            }
            catch (Exception e)
            {
                throw new PacketEncodeException(e.Message);
            }

            byte[] buf = packet.GetBuffer();
            UploadBytes += (ulong) await SendAsync(buf, buf.Length, packet.EndPoint);

            new ClientPacketSendEventArgs(this, packet, UploadBytes)
                .Invoke(this, SendPacketEvent);

            packet.Close();
        }

        public async void SendRawPacket(RawPacket packet)
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