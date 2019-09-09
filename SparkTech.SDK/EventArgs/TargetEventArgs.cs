namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class TargetEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly IAttackable Target;

        public TargetEventArgs(IUnit source, IAttackable target)
        {
            this.Source = source;

            this.Target = target;
        }
    }
}