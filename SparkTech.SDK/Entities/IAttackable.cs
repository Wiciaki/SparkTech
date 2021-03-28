namespace SparkTech.SDK.Entities
{
    public interface IAttackable : IGameObject
    {
        float Health { get; }
        float HealthPercent { get; }
        float MaxHealth { get; }
        float Mana { get; }
        float ManaPercent { get; }
        float MaxMana { get; }
        string ArmorMaterial { get; }
        float AllShield { get; }
        float PhysicalShield { get; }
        float MagicalShield { get; }
        bool IsPhysicalImmune { get; }
        bool IsMagicalImmune { get; }
        bool IsLifestealImmune { get; }
        bool IsVisible { get; }
        bool IsInvulnerable { get; }
        bool IsTargetable { get; }
        float PathfindingCollisionRadius { get; }
        float OverrideCollisionHeight { get; }
        float OverrideCollisionRadius { get; }
    }
}