namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class AggroEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly int TargetId;

        public AggroEventArgs(IUnit source, int targetId)
        {
            this.Source = source;

            this.TargetId = targetId;
        }
    }
}