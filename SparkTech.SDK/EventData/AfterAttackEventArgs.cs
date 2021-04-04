namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class AfterAttackEventArgs : EventArgs, IEventArgsTarget<IAttackable>
    {
        public int TargetId => this.Target.Id;

        public IAttackable Target { get; }

        public AfterAttackEventArgs(IAttackable target)
        {
            this.Target = target;
        }
    }
}