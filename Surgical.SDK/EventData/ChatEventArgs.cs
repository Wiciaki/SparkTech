namespace Surgical.SDK.EventData
{
    using Surgical.SDK.Entities;

    public class ChatEventArgs : BlockableEventArgs, IEventArgsSource<IHero>
    {
        public IHero Source { get; }

        public string Message { get; }

        public ChatEventArgs(IHero source, string message)
        {
            this.Source = source;

            this.Message = message;
        }
    }
}