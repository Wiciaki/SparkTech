namespace Surgical.SDK.Orbwalking
{
    using Surgical.SDK.Entities;

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