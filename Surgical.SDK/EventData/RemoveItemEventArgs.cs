namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class RemoveItemEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public int Slot { get; }

        public RemoveItemEventArgs(IUnit source, int slot)
        {
            this.Source = source;

            this.Slot = slot;
        }
    }
}