namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class AfterAttackEventArgs : EventArgs, IEventArgsTarget<IAttackable>
    {
        public int TargetId { get; }

        public IAttackable Target => ObjectManager.GetById<IAttackable>(this.TargetId);

        public AfterAttackEventArgs(int targetId)
        {
            this.TargetId = targetId;
        }
    }
}