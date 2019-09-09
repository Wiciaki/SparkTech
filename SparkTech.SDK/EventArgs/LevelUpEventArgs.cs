namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class LevelUpEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public LevelUpEventArgs(IUnit source)
        {
            this.Source = source;
        }
    }
}