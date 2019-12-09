namespace Surgical.SDK.EventData
{
    using Surgical.SDK.Entities;

    public class AfterAttackEventArgs : IEventArgsTarget<IAttackable>
    {
        public IAttackable Target { get; }

        public AfterAttackEventArgs(IAttackable target)
        {
            this.Target = target;
        }
    }
}