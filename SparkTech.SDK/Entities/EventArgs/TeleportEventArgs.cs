namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

    public class TeleportEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly string RecallType, RecallName;

        public TeleportEventArgs(IUnit source, string recallType, string recallName)
        {
            this.Source = source;

            this.RecallType = recallType;

            this.RecallName = recallName;
        }
    }
}