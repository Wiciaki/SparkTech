namespace SparkTech.Ensoul.Entities
{
    using SDK.Entities;

    public class AttackableUnit<TEntity> : GameObject<TEntity>, IAttackable where TEntity : EnsoulSharp.AttackableUnit
    {
        public AttackableUnit(TEntity entity) : base(entity)
        { }

        public float Health => this.Entity.Health;
        public float HealthPercent => this.Entity.HealthPercent;
        public float MaxHealth => this.Entity.MaxHealth;
        public float Mana => this.Entity.Mana;
        public float ManaPercent => this.Entity.ManaPercent;
        public float MaxMana => this.Entity.MaxMana;
        public string ArmorMaterial => this.Entity.ArmorMaterial;
        public float AllShield => this.Entity.AllShield;
        public float PhysicalShield => this.Entity.PhysicalShield;
        public float MagicalShield => this.Entity.MagicalShield;
        public bool IsPhysicalImmune => this.Entity.IsPhysicalImmune;
        public bool IsMagicalImmune => this.Entity.IsMagicalImmune;
        public bool IsLifestealImmune => this.Entity.IsLifestealImmune;
        public bool IsInvulnerable => this.Entity.IsInvulnerable;
        public bool IsTargetable => this.Entity.IsTargetable;
        public bool IsVisible => this.Entity.IsVisible;
        public float PathfindingCollisionRadius => this.Entity.PathfindingCollisionRadius;
        public float OverrideCollisionHeight => this.Entity.OverrideCollisionHeight;
        public float OverrideCollisionRadius => this.Entity.OverrideCollisionRadius;
    }
}