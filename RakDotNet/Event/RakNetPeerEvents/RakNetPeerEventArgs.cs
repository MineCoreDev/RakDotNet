using System;
using RakDotNet.Server.Peer;

namespace RakDotNet.Event.RakNetPeerEvents
{
    public class RakNetPeerEventArgs : EventArgs
    {
        public RakNetPeer Peer { get; }

        public RakNetPeerEventArgs(RakNetPeer peer)
        {
            Peer = peer;
        }
    }
}