namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _PauseAnimationEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public int SourceId { get; }

        public IUnit Source => ObjectManager.GetById<IUnit>(this.SourceId);

        public bool Pause { get; }

        public _PauseAnimationEventArgs(int sourceId, bool pause)
        {
            this.SourceId = sourceId;
            this.Pause = pause;
        }
    }
}