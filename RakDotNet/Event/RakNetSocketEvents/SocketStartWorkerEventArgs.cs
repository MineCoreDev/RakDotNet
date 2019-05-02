namespace RakDotNet.Event.RakNetSocketEvents
{
    public class SocketStartWorkerEventArgs : RakNetSocketEventArgs
    {
        public SocketStartWorkerEventArgs(RakNetSocket socket) : base(socket)
        {
        }
    }
}