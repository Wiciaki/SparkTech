namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

    public class TargetEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly IAttackableUnit Target;

        public TargetEventArgs(IUnit source, IAttackableUnit target)
        {
            this.Source = source;

            this.Target = target;
        }
    }
}