namespace SparkTech.SDK.EventData
{
    using SparkTech.SDK.Entities;

    public class BeforeAttackEventArgs : BlockableEventArgs, IEventArgsTarget<IAttackable>
    {
        public int TargetId { get; }

        public IAttackable Target => ObjectManager.GetById<IAttackable>(this.TargetId);

        public BeforeAttackEventArgs(int targetId)
        {
            this.TargetId = targetId;
        }
    }
}