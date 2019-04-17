namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientStartWorkerEventArgs : RakNetClientEventArgs
    {
        public ClientStartWorkerEventArgs(RakNetSocket socket) : base(socket)
        {
        }
    }
}