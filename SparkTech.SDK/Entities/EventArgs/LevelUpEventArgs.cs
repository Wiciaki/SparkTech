namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

    public class LevelUpEventArgs : EventArgs, ISourcedEventArgs<IUnit>
    {
        public IUnit Source { get; }

        public LevelUpEventArgs(IUnit source)
        {
            this.Source = source;
        }
    }
}