using System.Threading;
using Open.Nat;

namespace RakDotNet
{
    public class RakNet
    {
        public static async void ForwardPort()
        {
            var nat = new NatDiscoverer();
            var token = new CancellationTokenSource(5000);
            var device = await nat.DiscoverDeviceAsync(PortMapper.Upnp, token);
            device.CreatePortMapAsync(new Mapping(Protocol.Udp, ))
        }
    }
}