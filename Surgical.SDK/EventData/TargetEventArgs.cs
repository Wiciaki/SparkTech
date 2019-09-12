namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class TargetEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public IAttackable Target { get; }

        public TargetEventArgs(IUnit source, IAttackable target)
        {
            this.Source = source;

            this.Target = target;
        }
    }
}