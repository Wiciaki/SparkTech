namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class PlaceItemInSlotEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly int Slot, Stack;

        public readonly int ItemId; // todo

        public PlaceItemInSlotEventArgs(IUnit source, int slot, int stack, int itemId)
        {
            this.Source = source;

            this.Slot = slot;

            this.Stack = stack;

            this.ItemId = itemId;
        }
    }
}