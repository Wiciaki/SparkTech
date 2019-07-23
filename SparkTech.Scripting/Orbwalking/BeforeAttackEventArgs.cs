namespace SparkTech.Orbwalking
{
    public class BeforeAttackEventArgs : EntropyEventArgs
    {
        public readonly AttackableUnit Target;

        public bool Process;

        public BeforeAttackEventArgs(AttackableUnit target)
        {
            this.Target = target;
        }
    }
}