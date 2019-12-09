namespace Surgical.SDK.EventData
{
    using Surgical.SDK.Entities;

    public class BeforeAttackEventArgs : BlockableEventArgs, IEventArgsTarget<IAttackable>
    {
        public IAttackable Target { get; }

        public BeforeAttackEventArgs(IAttackable target)
        {
            this.Target = target;
        }
    }
}