namespace SparkTech.SDK.EventData
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public class _PingEventArgs : BlockableEventArgs, IEventArgsSource<IGameObject>, IEventArgsTarget<IGameObject>
    {
        public int TargetId { get; }

        public int SourceId { get; }

        public IGameObject Source => ObjectManager.GetById(this.SourceId);

        public IGameObject Target => ObjectManager.GetById(this.TargetId);

        public Vector2 Position { get; }

        public PingCategory PingCategory { get; }

        public _PingEventArgs(int sourceId, int targetId, Vector2 position, PingCategory pingCategory)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
            this.Position = position;
            this.PingCategory = pingCategory;
        }
    }
}