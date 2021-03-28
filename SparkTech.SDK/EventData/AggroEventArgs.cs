namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class AggroEventArgs : EventArgs, IEventArgsSource<IUnit>, IEventArgsTarget<IAttackable>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public int TargetId { get; }

        public IAttackable Target => ObjectManager.GetById<IAttackable>(this.TargetId);

        public AggroEventArgs(int sourceId, int targetId)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
        }
    }
}