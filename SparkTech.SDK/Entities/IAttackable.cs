namespace SparkTech.SDK.Entities
{
    public interface IAttackable : IGameObject
    {
        float Health { get; }
        float MaxHealth { get; }

        float Mana { get; }
        float MaxMana { get; }

        float PhysicalShield { get; }
        float MagicalShield { get; }

        bool PhysicalImmune { get; }
        bool MagicImmune { get; }
        bool LifestealImmune { get; }

        bool IsInvulnerable { get; }
        bool IsTargetable { get; }
        bool IsZombie { get; }

        // float PathfindingCollisionRadius
        // float OverrideCollisionHeight
        // float OverrideCollisionRadius
    }
}