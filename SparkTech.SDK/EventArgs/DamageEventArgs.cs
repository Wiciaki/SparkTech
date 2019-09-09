namespace SparkTech.SDK.EventArgs
{
    using System;

    using SparkTech.SDK.Entities;

    public class DamageEventArgs : EventArgs, ISourcedEventArgs<IAttackable>
    {
        public IAttackable Source { get; }

        public readonly IAttackable Target;

        public readonly DamageHitType HitType;

        public readonly DamageType DamageType;

        public readonly float Damage;

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