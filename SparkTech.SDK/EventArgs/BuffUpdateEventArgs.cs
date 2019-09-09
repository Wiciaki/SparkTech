namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class BuffUpdateEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly IBuff Buff;

        public BuffUpdateEventArgs(IUnit source, IBuff buff)
        {
            this.Source = source;

            this.Buff = buff;
        }
    }
}