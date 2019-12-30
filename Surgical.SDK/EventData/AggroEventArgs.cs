namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class AggroEventArgs : EventArgs, IEventArgsSource<IUnit>, IEventArgsTarget<IAttackable>
    {
        public IUnit Source { get; }

        public IAttackable Target { get; }

        public AggroEventArgs(IUnit source, IAttackable target)
        {
            this.Source = source;
            this.Target = target;
        }
    }
}