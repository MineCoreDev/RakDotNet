namespace RakDotNet.Event
{
    public class CancelableEventArgs : ICancelableEvent
    {
        public bool IsCanceled { get; set; }
    }
}