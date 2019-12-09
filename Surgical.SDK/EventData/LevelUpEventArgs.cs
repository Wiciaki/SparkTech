namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class LevelUpEventArgs : EventArgs, IEventArgsSource<IUnit>
    {
        public IUnit Source { get; }

        public LevelUpEventArgs(IUnit source)
        {
            this.Source = source;
        }
    }
}