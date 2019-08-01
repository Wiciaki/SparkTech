namespace SparkTech.SDK.Entities
{
    using System;

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

        //Action<OnDamageEventArgs> OnDamage { get; set; }

        //Action<IAttackableUnit> OnLeaveVisiblityClient { get; set; }
        
        OnLeaveTeamVisiblity
        // OnLeaveLocalVisiblityClient
        // ^x2 for enter
    }
}