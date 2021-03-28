namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _PlaceItemInSlotEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public int Slot { get; }

        public int Stack { get; }

        public ItemId ItemId  { get; }

        public _PlaceItemInSlotEventArgs(int sourceId, int slot, int stack, ItemId itemId)
        {
            this.SourceId = sourceId;
            this.Slot = slot;
            this.Stack = stack;
            this.ItemId = itemId;
        }
    }
}