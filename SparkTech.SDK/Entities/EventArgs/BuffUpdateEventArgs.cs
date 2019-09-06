namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

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