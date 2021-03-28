namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IUnit : IAttackable
    {
        string SkinName { get; }
        string CharName { get; }

        bool CanAttack { get; }
        bool CanCast { get; }
        bool CanMove { get; }
        bool CanWalk { get; }

        bool IsWindingUp { get; }
        bool IsMoving { get; }
        bool IsAsleep { get; }
        bool IsCharmed { get; }
        bool IsClickproofToEnemies { get; }
        bool IsCursorAllowedWhileUntargetable { get; }
        bool IsDodgingMissiles { get; }
        bool IsFeared { get; }
        bool IsFleeing { get; }
        bool IsForceRenderParticles { get; }
        bool IsGhosted { get; }
        bool IsGhostedForAllies { get; }
        bool IsGrounded { get; }
        bool IsNearSight { get; }
        bool IsNoRender { get; }
        bool IsObscured { get; }
        bool IsRevealSpecificUnit { get; }
        bool IsSelectable { get; }
        bool IsSlowed { get; }
        bool IsStealthed { get; }
        bool IsStunned { get; }
        bool IsSuppressed { get; }
        bool IsTaunted { get; }

        float AbilityHasteMod { get; }
        float PercentCooldownMod { get; }
        float FlatMagicDamageMod { get; }
        float PercentMagicDamageMod { get; }
        float BaseAbilityDamage { get; }
        float FlatPhysicalDamageMod { get; }
        float PercentPhysicalDamageMod { get; }
        float PercentBonusPhysicalDamageMod { get; }
        float PercentBasePhysicalDamageAsFlatBonusMod { get; }
        float BaseAttackDamage { get; }
        float PercentLifeStealMod { get; }
        float PercentAttackSpeedMod { get; }
        float AttackSpeedMod { get; }
        float FlatCritChanceMod { get; }
        float FlatCritDamageMod { get; }
        float PercentCritDamageMod { get; }
        float CritDamageMultiplier { get; }
        float Crit { get; }
        float Armor { get; }
        float BonusArmor { get; }
        float SpellBlock { get; }
        float BonusSpellBlock { get; }
        float BasePARRegenRate { get; }
        float PARRegenRate { get; }
        float BaseHPRegenRate { get; }
        float HPRegenRate { get; }
        float MoveSpeed { get; }
        float AttackRange { get; }
        float FlatArmorPenetrationMod { get; }
        float FlatMagicPenetrationMod { get; }
        float PercentArmorPenetrationMod { get; }
        float PercentMagicPenetrationMod { get; }
        float PercentBonusArmorPenetrationMod { get; }
        float PercentBonusMagicPenetrationMod { get; }
        float PhysicalLethality { get; }
        float MagicLethality { get; }
        float TotalAttackDamage { get; }
        float TotalMagicalDamage { get; }
        float BaseHP { get; }
        float BaseMoveSpeed { get; }

        float ExpGiveRadius { get; }
        float Gold { get; }
        float GoldTotal { get; }
            
        float BonusHealth { get; }
        float BonusMana { get; }

        float AttackDelay { get; }
        float AttackCastDelay { get; }

        Vector2 HPBarPosition { get; }
        bool IsHPBarRendered { get; }

        GameObjectCombatType CombatType { get; }
        Vector3 Direction { get; }
        Vector3 Velocity { get; }
        Vector3 InfoComponentBasePosition { get; }

        ISpellbook Spellbook { get; }
        ISpellData BasicAttack { get; }
        IBuff[] Buffs { get; }
        IItem[] InventoryItems { get; }
        IGameObject Pet { get; }

        bool HasBuff(string buffName);
        bool HasBuffOfType(BuffType buff);
        IBuff GetBuff(string buffName);
        int GetBuffCount(string buffName);

        Vector3[] Path { get; }
        Vector3[] GetPath(Vector3 end, bool smoothPath = false);
        Vector3[] GetPath(Vector3 start, Vector3 end, bool smoothPath = false);

        int SkinId { get; }
        void SetSkin(string model, int id);

        bool CanUseObject(IAttackable target);
        bool UseObject(IAttackable target);
        float GetAbilityResource(AbilityResourceType abilityResourceType, OutputType outputType);


    }
}