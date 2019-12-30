namespace Surgical.SDK.EventData
{
    using Surgical.SDK.Entities;

    public interface IEventArgsTarget<out T> where T : class, IGameObject
    {
        T? Target { get; }
    }
}