namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _VisibilityChangedEventArgs : EventArgs, IEventArgsSource<IAttackable>
    {
        public IAttackable Source { get; }

        public _VisibilityChangedEventArgs(int sourceId) : this(ObjectManager.GetById<IAttackable>(sourceId))
        { }

        public _VisibilityChangedEventArgs(IAttackable source)
        {
            this.Source = source;
        }
    }
}