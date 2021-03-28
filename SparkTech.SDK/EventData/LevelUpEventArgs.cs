namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class LevelUpEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public int Level { get; }

        public LevelUpEventArgs(int sourceId, int level)
        {
            this.SourceId = sourceId;
            this.Level = level;
        }
    }
}