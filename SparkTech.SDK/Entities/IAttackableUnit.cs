namespace SparkTech.SDK.Entities
{
    public interface IAttackableUnit : IGameObject
    {
        float Health();
        float MaxHealth();

        float Mana();
        float MaxMana();

        float PhysicalShield();
        float MagicalShield();

        bool PhysicalImmune();
        bool MagicImmune();

        bool IsInvulnerable();
        bool IsTargetable();
        bool IsZombie();

        // ?

        bool LifestealImmune();

        // float PathfindingCollisionRadius
        // float OverrideCollisionHeight
        // float OverrideCollisionRadius
    }
}