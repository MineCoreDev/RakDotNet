using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Open.Nat;
using RakDotNet.Utils;

namespace RakDotNet
{
    public class RakNet
    {
        public const int SERVER_NETWORK_PROTOCOL = 9;
        public const int CLIENT_NETWORK_PROTOCOL = 9;
        
        private static NatDiscoverer _natDiscoverer;
        private static NatDevice _natDevice;

        static RakNet()
        {
            InitNatDevice();
        }

        public static async void ForwardPort(UPnPSettings settings)
        {
            await _natDevice.CreatePortMapAsync(new Mapping(Protocol.Udp,
                settings.PrivatePort, settings.PublicPort, settings.LifeTime, "RakNet UPnP"));
        }

        public static async void ClosePort(UPnPSettings settings)
        {
            await _natDevice.DeletePortMapAsync(new Mapping(Protocol.Udp,
                settings.PrivatePort, settings.PublicPort, settings.LifeTime, "RakNet UPnP"));
        }

        public static async Task<bool> IsForwardedPort(ushort port)
        {
            return await _natDevice.GetSpecificMappingAsync(Protocol.Udp, port) != null;
        }

        public static RakNetSocket CreateClient(IPEndPoint endPoint)
        {
            return new RakNetSocket(endPoint);
        }

        private static async void InitNatDevice()
        {
            _natDiscoverer = new NatDiscoverer();
            var token = new CancellationTokenSource(5000);
            _natDevice = await _natDiscoverer.DiscoverDeviceAsync(PortMapper.Upnp, token);
        }
    }
}