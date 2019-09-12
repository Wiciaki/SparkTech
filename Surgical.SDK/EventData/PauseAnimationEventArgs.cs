namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class PauseAnimationEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public bool Pause { get; }

        public PauseAnimationEventArgs(IUnit source, bool pause)
        {
            this.Source = source;

            this.Pause = pause;
        }
    }
}