namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientStartWorkerEventArgs : RakNetClientEventArgs
    {
        public ClientStartWorkerEventArgs(RakNetClient client) : base(client)
        {
        }
    }
}