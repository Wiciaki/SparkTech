namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

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