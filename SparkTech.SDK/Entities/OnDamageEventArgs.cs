namespace SparkTech.SDK.Entities
{
    public class OnDamageEventArgs
    {
        public readonly float Damage;

        public readonly int SourceId;

        public readonly int TargetId;

        public readonly DamageHitType HitType;

        public readonly DamageType DamageType;

        public OnDamageEventArgs(int sourceId, int targetId, float damage, DamageHitType hitType, DamageType damageType)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
            this.Damage = damage;
            this.HitType = hitType;
            this.DamageType = damageType;
        }
    }
}