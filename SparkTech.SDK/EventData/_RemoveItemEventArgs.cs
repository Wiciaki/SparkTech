namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _RemoveItemEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public int Slot { get; }

        public _RemoveItemEventArgs(int sourceId, int slot)
        {
            this.SourceId = sourceId;
            this.Slot = slot;
        }
    }
}