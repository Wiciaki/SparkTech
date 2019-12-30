namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class BuffUpdateEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public IBuff Buff { get; }

        public BuffUpdateEventArgs(IUnit source, IBuff buff)
        {
            this.Source = source;
            this.Buff = buff;
        }
    }
}