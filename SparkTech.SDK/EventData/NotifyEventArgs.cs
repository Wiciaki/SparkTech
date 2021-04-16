namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.League;

    public class NotifyEventArgs : EventArgs, IEventArgsSource<IGameObject>
    {
        public int SourceId { get; }

        public IGameObject Source => ObjectManager.GetById(this.SourceId);

        public GameEventId EventId { get; }

        public NotifyEventArgs(int sourceId, GameEventId eventId)
        {
            this.SourceId = sourceId;
            this.EventId = eventId;
        }
    }
}