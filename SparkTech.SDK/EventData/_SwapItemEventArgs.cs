namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _SwapItemEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public int From { get; }

        public int To { get; }

        public _SwapItemEventArgs(int sourceId, int from, int to)
        {
            this.SourceId = sourceId;
            this.From = from;
            this.To = to;
        }
    }
}