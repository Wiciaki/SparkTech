namespace SparkTech.SDK.EventData
{
    using SparkTech.SDK.Entities;

    public interface IEventArgsTarget<out T> where T : class, IGameObject
    {
        int TargetId { get; }

        T Target { get; }
    }
}