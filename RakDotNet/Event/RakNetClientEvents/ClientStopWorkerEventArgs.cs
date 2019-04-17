namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientStopWorkerEventArgs : RakNetClientEventArgs
    {
        public ClientStopWorkerEventArgs(RakNetSocket socket) : base(socket)
        {
        }
    }
}