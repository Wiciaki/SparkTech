namespace Surgical.SDK.EventData
{
    public interface IEventArgsSource<out T> where T : class
    {
        T Source { get; }
    }
}