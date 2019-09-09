namespace SparkTech.SDK.Orbwalking
{
    using SparkTech.SDK.Entities;

    public class BeforeAttackEventArgs
    {
        public readonly IAttackable Target;

        public bool Process;

        public BeforeAttackEventArgs(IAttackable target)
        {
            this.Target = target;
        }
    }
}