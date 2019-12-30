namespace Surgical.SDK.EventData
{
    using SharpDX;

    using Surgical.SDK.Entities;

    public class PingEventArgs : BlockableEventArgs, IEventArgsSource<IGameObject>, IEventArgsTarget<IGameObject>
    {
        public IGameObject Source { get; }

        public IGameObject? Target { get; }

        public Vector2 Position { get; }

        public PingCategory PingCategory { get; }

        public PingEventArgs(IGameObject source, IGameObject target, Vector2 position, PingCategory pingCategory)
        {
            this.Source = source;
            this.Target = target;
            this.Position = position;
            this.PingCategory = pingCategory;
        }
    }
}