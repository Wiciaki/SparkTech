namespace SparkTech.SDK.EventData
{
    using SparkTech.SDK.Entities;

    public class BeforeAttackEventArgs : BlockableEventArgs, IEventArgsTarget<IAttackable>
    {
        public int TargetId => this.Target.Id;

        public IAttackable Target { get; }

        public BeforeAttackEventArgs(IAttackable target)
        {
            this.Target = target;
        }
    }
}