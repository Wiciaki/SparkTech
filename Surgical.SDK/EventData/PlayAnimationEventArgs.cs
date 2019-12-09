namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class PlayAnimationEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public string Animation { get; }

        public PlayAnimationEventArgs(IUnit source, string animation)
        {
            this.Source = source;

            this.Animation = animation;
        }
    }
}