namespace Surgical.SDK.EventData
{
    public class NotifyEventArgs
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