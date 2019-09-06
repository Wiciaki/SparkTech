namespace SparkTech.SDK.Entities.EventArgs
{
    using System;

    public class DamageEventArgs : EventArgs, ISourcedEventArgs<IAttackableUnit>
    {
        public IAttackableUnit Source { get; }

        public readonly IAttackableUnit Target;

        public readonly DamageHitType HitType;

        public readonly DamageType DamageType;

        public readonly float Damage;

        public DamageEventArgs(IAttackableUnit source, IAttackableUnit target, DamageHitType hitType, DamageType damageType, float damage)
        {
            this.Source = source;

            this.Target = target;

            this.Damage = damage;

            this.HitType = hitType;

            this.DamageType = damageType;
        }
    }
}