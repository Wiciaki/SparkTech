namespace Surgical.SDK.EventData
{
    public interface ISourcedEventArgs<out T>
    {
        T Source { get; }
    }
}