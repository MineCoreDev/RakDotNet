namespace RakDotNet.Event.RakNetSocketEvents
{
    public class SocketStopWorkerEventArgs : RakNetSocketEventArgs
    {
        public SocketStopWorkerEventArgs(RakNetSocket socket) : base(socket)
        {
        }
    }
}