namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class TeleportEventArgs : EventArgs, IEventArgsSource<IAttackable>
    {
        public int SourceId { get; }

        public IAttackable Source => ObjectManager.GetById<IAttackable>(this.SourceId);

        public string RecallType { get; }

        public string RecallName { get; }

        public TeleportEventArgs(int sourceId, string recallType, string recallName)
        {
            this.SourceId = sourceId;
            this.RecallType = recallType;
            this.RecallName = recallName;
        }
    }
}