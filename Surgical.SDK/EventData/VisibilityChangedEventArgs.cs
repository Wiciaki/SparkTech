namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class VisibilityChangedEventArgs : EventArgs, IEventArgsSource<IAttackable>
    {
        public IAttackable Source { get; }

        public VisibilityChangedEventArgs(IAttackable source)
        {
            this.Source = source;
        }
    }
}