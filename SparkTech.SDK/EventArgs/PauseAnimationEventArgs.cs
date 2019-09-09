namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class PauseAnimationEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly bool Pause;

        public PauseAnimationEventArgs(IUnit source, bool pause)
        {
            this.Source = source;

            this.Pause = pause;
        }
    }
}