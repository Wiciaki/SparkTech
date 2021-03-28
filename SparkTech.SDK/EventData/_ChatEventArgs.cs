namespace SparkTech.SDK.EventData
{
    using SparkTech.SDK.Entities;

    public class _ChatEventArgs : BlockableEventArgs, IEventArgsSource<IHero>
    {
        public int SourceId { get; }

        public IHero Source => ObjectManager.GetById<IHero>(this.SourceId);

        public string Message { get; }

        public _ChatEventArgs(int sourceId, string message)
        {
            this.SourceId = sourceId;
            this.Message = message;
        }
    }
}