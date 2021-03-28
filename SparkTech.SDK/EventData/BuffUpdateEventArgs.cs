namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class BuffUpdateEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public IBuff Buff { get; }

        public BuffUpdateEventArgs(int sourceId, IBuff buff)
        {
            this.SourceId = sourceId;
            this.Buff = buff;
        }
    }
}