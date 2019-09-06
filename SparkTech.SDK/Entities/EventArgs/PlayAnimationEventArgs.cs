namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

    public class PlayAnimationEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public readonly string Animation;

        public PlayAnimationEventArgs(IUnit source, string animation)
        {
            this.Source = source;

            this.Animation = animation;
        }
    }
}