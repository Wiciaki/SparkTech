namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class TeleportEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public string RecallType { get; }

        public string RecallName { get; }

        public TeleportEventArgs(IUnit source, string recallType, string recallName)
        {
            this.Source = source;

            this.RecallType = recallType;

            this.RecallName = recallName;
        }
    }
}