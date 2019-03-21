using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Open.Nat;

namespace RakDotNet
{
    public class RakNet
    {
        private static NatDiscoverer _natDiscoverer;
        private static NatDevice _natDevice;

        static RakNet()
        {
            InitNatDevice();
        }

        public static async void ForwardPort(UpnpPort port)
        {
            await _natDevice.CreatePortMapAsync(new Mapping(Protocol.Udp,
                port.PrivatePort, port.PublicPort, port.LifeTime, "RakNet UPnP"));
        }

        public static async void ClosePort(UpnpPort port)
        {
            await _natDevice.DeletePortMapAsync(new Mapping(Protocol.Udp,
                port.PrivatePort, port.PublicPort, port.LifeTime, "RakNet UPnP"));
        }

        public static async Task<bool> IsForwardedPort(ushort port)
        {
            return await _natDevice.GetSpecificMappingAsync(Protocol.Udp, port) != null;
        }

        public static RakNetClient StartClient(IPEndPoint endPoint)
        {
            return new RakNetClient(endPoint);
        }

        private static async void InitNatDevice()
        {
            _natDiscoverer = new NatDiscoverer();
            var token = new CancellationTokenSource(5000);
            _natDevice = await _natDiscoverer.DiscoverDeviceAsync(PortMapper.Upnp, token);
        }
    }
}