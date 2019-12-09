namespace Surgical.SDK.EventData
{
    public interface IEventArgsSource<out T>
    {
        T Source { get; }
    }
}