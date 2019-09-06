namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

    public class SwapItemEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly int From, To;

        public SwapItemEventArgs(IUnit source, int from, int to)
        {
            this.Source = source;

            this.From = from;

            this.To = to;
        }
    }
}