namespace RakDotNet.Event.RakNetClientEvents
{
    public class ClientStopWorkerEventArgs : RakNetClientEventArgs
    {
        public ClientStopWorkerEventArgs(RakNetClient client) : base(client)
        {
        }
    }
}