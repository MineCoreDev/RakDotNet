using RakDotNet.Server.Peer;

namespace RakDotNet.Event.RakNetPeerEvents
{
    public class PeerTimedOutEventArgs : RakNetPeerEventArgs
    {
        public PeerTimedOutEventArgs(RakNetPeer peer) : base(peer)
        {
        }
    }
}