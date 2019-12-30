namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class PlaceItemInSlotEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public int Slot { get; }

        public int Stack { get; }

        public int ItemId  { get; } // todo

        public PlaceItemInSlotEventArgs(IUnit source, int slot, int stack, int itemId)
        {
            this.Source = source;
            this.Slot = slot;
            this.Stack = stack;
            this.ItemId = itemId;
        }
    }
}