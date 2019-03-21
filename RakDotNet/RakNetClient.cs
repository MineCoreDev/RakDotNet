using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RakDotNet.Protocols;
using RakDotNet.Utils;

namespace RakDotNet
{
    public class RakNetClient : UdpClient
    {
        public ulong DownloadBytes { get; private set; }
        public ulong UploadBytes { get; private set; }

        public Task NetworkWorker { get; }
        public CancellationTokenSource WorkerCancelToken { get; }

        public Action<Packet> OnReceive { private get; set; }

        public RakNetClient(IPEndPoint endPoint, bool useWorker = false) : base(endPoint)
        {
            if (useWorker)
            {
                WorkerCancelToken = new CancellationTokenSource();
                NetworkWorker = Task.Factory.StartNew(async () =>
                {
                    if (WorkerCancelToken.IsCancellationRequested)
                    {
                        Logger.Debug($"{NetworkWorker.Id} is Canceled.");
                        WorkerCancelToken.Token.ThrowIfCancellationRequested();
                    }

                    OnReceive?.Invoke(await ReceivePacket());
                }, WorkerCancelToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                NetworkWorker.Start();
            }
        }

        public async Task<Packet> ReceivePacket()
        {
            UdpReceiveResult result = await ReceiveAsync();
            Packet packet = new Packet(result.Buffer);
            packet.EndPoint = packet.EndPoint;
            DownloadBytes += (uint) result.Buffer.Length;
            packet.DecodeHeader();
            packet.DecodePayload();
            return packet;
        }

        public async void SendPacket(Packet packet)
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