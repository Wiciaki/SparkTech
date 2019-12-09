namespace Surgical.SDK.EventData
{
    using System;

    using Surgical.SDK.Entities;

    public class DamageEventArgs : EventArgs, IEventArgsSource<IAttackable>, IEventArgsTarget<IAttackable>
    {
        public IAttackable Source { get; }

        public IAttackable Target { get; }

        public DamageHitType HitType { get; }

        public DamageType DamageType { get; }

        public float Damage { get; }

        public DamageEventArgs(IAttackable source, IAttackable target, DamageHitType hitType, DamageType damageType, float damage)
        {
            this.Source = source;

            this.Target = target;

            this.Damage = damage;

            this.HitType = hitType;

            this.DamageType = damageType;
        }
    }
}