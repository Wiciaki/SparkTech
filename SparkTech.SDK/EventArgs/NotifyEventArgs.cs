namespace SparkTech.SDK.EventArgs
{
    public class NotifyEventArgs
    {
        public readonly int SenderId;

        public readonly GameEvent Event;

        public NotifyEventArgs(int senderId, GameEvent @event)
        {
            this.SenderId = senderId;

            this.Event = @event;
        }
    }
}