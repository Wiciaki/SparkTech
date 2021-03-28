namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.Entities;

    public class _DamageEventArgs : EventArgs, IEventArgsSource<IAttackable>, IEventArgsTarget<IAttackable>
    {
        public int SourceId { get; }

        public IAttackable Source => ObjectManager.GetById<IAttackable>(this.SourceId);

        public int TargetId { get; }

        public IAttackable Target => ObjectManager.GetById<IAttackable>(this.TargetId);

        public DamageHitType HitType { get; }

        public DamageType DamageType { get; }

        public float Damage { get; }

        public _DamageEventArgs(int sourceId, int targetId, DamageHitType hitType, DamageType damageType, float damage)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
            this.Damage = damage;
            this.HitType = hitType;
            this.DamageType = damageType;
        }
    }
}