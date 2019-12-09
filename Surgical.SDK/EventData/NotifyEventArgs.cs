namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class NotifyEventArgs : EventArgs, IEventArgsSource<IGameObject>
    {
        public IGameObject Source { get; }

        public GameEvent Event { get; }

        public NotifyEventArgs(IGameObject source, GameEvent @event)
        {
            this.Source = source;

            this.Event = @event;
        }
    }
}