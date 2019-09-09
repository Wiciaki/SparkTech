namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class RemoveItemEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly int Slot;

        public RemoveItemEventArgs(IUnit source, int slot)
        {
            this.Source = source;

            this.Slot = slot;
        }
    }
}