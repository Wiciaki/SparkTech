namespace Surgical.SDK.EventData
{
    using System;

    public class NotifyEventArgs : EventArgs
    {
        public int SenderId { get; } // todo

        public GameEvent Event { get; }

        public NotifyEventArgs(int senderId, GameEvent @event)
        {
            this.SenderId = senderId;

            this.Event = @event;
        }
    }
}