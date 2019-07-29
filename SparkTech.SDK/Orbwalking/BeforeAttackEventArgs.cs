namespace SparkTech.SDK.Orbwalking
{
    using SparkTech.SDK.Entities;

    public class BeforeAttackEventArgs
    {
        public readonly IAttackableUnit Target;

        public bool Process;

        public BeforeAttackEventArgs(IAttackableUnit target)
        {
            this.Target = target;
        }
    }
}