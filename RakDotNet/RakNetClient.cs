using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RakDotNet.Protocols;
using RakDotNet.Protocols.Packets;
using RakDotNet.Utils;

namespace RakDotNet
{
    public class RakNetClient : UdpClient
    {
        public PacketIdentifier PacketIdentifier { get; }

        public ulong DownloadBytes { get; private set; }
        public ulong UploadBytes { get; private set; }

        public Task NetworkWorker { get; private set; }
        public CancellationTokenSource WorkerCancelToken { get; private set; }

        public Action<Packet> OnReceive { private get; set; }

        public RakNetClient(IPEndPoint endPoint) : base(endPoint)
        {
            PacketIdentifier = new PacketIdentifier();
        }

        public void StartReceiveWorker()
        {
            WorkerCancelToken = new CancellationTokenSource();
            NetworkWorker = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (!WorkerCancelToken.IsCancellationRequested)
                    {
                        Logger.Debug($"{NetworkWorker.Id} is Canceled.");
                        WorkerCancelToken.Token.ThrowIfCancellationRequested();
                    }

                    OnReceive?.Invoke(await ReceivePacket());
                }
            }, WorkerCancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task<RakNetPacket> ReceivePacket()
        {
            UdpReceiveResult result = await ReceiveAsync();
            byte[] buffer = result.Buffer;
            RakNetPacket packet = PacketIdentifier.GetPacketFormId(buffer[0]);
            packet.SetBuffer(buffer);
            packet.EndPoint = packet.EndPoint;
            DownloadBytes += (uint) result.Buffer.Length;

            packet.DecodeHeader();
            packet.DecodePayload();

            return packet;
        }

        public async void SendPacket(RakNetPacket packet)
        {
            packet.EncodeHeader();
            packet.EncodePayload();

            byte[] buf = packet.ToArray();
            UploadBytes += (ulong) await SendAsync(buf, buf.Length, packet.EndPoint);
            packet.Close();
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