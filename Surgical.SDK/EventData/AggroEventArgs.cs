namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class AggroEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public int TargetId { get; }

        public AggroEventArgs(IUnit source, int targetId)
        {
            this.Source = source;

            this.TargetId = targetId;
        }
    }
}