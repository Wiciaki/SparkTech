namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class SwapItemEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public int From { get; }

        public int To { get; }

        public SwapItemEventArgs(IUnit source, int from, int to)
        {
            this.Source = source;
            this.From = from;
            this.To = to;
        }
    }
}