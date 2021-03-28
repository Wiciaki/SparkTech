namespace SparkTech.SDK.EventData
{
    using SparkTech.SDK.Entities;

    public class PlayAnimationEventArgs : BlockableEventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source { get; }

        public string Animation { get; }

        public PlayAnimationEventArgs(int sourceId, string animation)
        {
            this.SourceId = sourceId;
            this.Animation = animation;
        }
    }
}