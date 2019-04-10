namespace RakDotNet.Event
{
    public interface ICancelableEvent
    {
        bool IsCanceled { get; set; }
    }
}