namespace SparkTech.Ensoul.Entities
{
    using System;

    using SDK.Entities;

    using SharpDX;

    using SparkTech.Ensoul.Fragments;

    public class AIBaseClient<TEntity> : AttackableUnit<TEntity>, IUnit where TEntity : EnsoulSharp.AIBaseClient
    {
        public AIBaseClient(TEntity entity) : base(entity)
        { }

        public int SkinId => this.Entity.SkinId;
        public string SkinName => this.Entity.SkinName;
        public string CharName => this.Entity.CharacterName;
        public bool IsWindingUp => this.Entity.IsWindingUp;
        public float ExpGiveRadius => this.Entity.ExpGiveRadius;
        public float Gold => this.Entity.Gold;
        public GameObjectCombatType CombatType => (GameObjectCombatType)this.Entity.CombatType;
        public bool IsMoving => this.Entity.IsMoving;
        public Vector3[] Path => this.Entity.Path;
        public ISpellbook Spellbook => EntityConverter.Convert(this.Entity.Spellbook);
        public bool CanAttack => this.Entity.CanAttack;
        public bool CanCast => this.Entity.CanCast;
        public bool CanMove => this.Entity.CanMove;
        public bool IsCharmed => this.Entity.IsCharmed;
        public IBuff[] Buffs => Array.ConvertAll(this.Entity.Buffs, EntityConverter.Convert);
        public IGameObject Pet => ObjectManagerFragment.Convert(this.Entity.Pet);
        public float AttackDelay => this.Entity.AttackDelay;
        public float AttackCastDelay => this.Entity.AttackCastDelay;
        public Vector2 HPBarPosition => this.Entity.HPBarPosition;
        public bool IsHPBarRendered => this.Entity.IsHPBarRendered;
        public Vector3 Direction => this.Entity.Direction;

        public bool CanWalk => this.Entity.CanWalk;
        public bool IsAsleep => this.Entity.IsAsleep;
        public bool IsClickproofToEnemies => this.Entity.IsClickproofToEnemies;
        public bool IsCursorAllowedWhileUntargetable => this.Entity.IsCursorAllowedWhileUntargetable;
        public bool IsDodgingMissiles => this.Entity.IsDodgingMissiles;
        public bool IsFeared => this.Entity.IsFeared;
        public bool IsFleeing => this.Entity.IsFleeing;
        public bool IsForceRenderParticles => this.Entity.IsForceRenderParticles;
        public bool IsGhosted => this.Entity.IsGhosted;
        public bool IsGhostedForAllies => this.Entity.IsGhostedForAllies;
        public bool IsGrounded => this.Entity.IsGrounded;
        public bool IsNearSight => this.Entity.IsNearSight;
        public bool IsNoRender => this.Entity.IsNoRender;
        public bool IsObscured => this.Entity.IsObscured;
        public bool IsRevealSpecificUnit => this.Entity.IsRevealSpecificUnit;
        public bool IsSelectable => this.Entity.IsSelectable;
        public bool IsSlowed => this.Entity.IsSlowed;
        public bool IsStealthed => this.Entity.IsStealthed;
        public bool IsStunned => this.Entity.IsStunned;
        public bool IsSuppressed => this.Entity.IsSuppressed;
        public bool IsTaunted => this.Entity.IsTaunted;
        public float AbilityHasteMod => this.Entity.AbilityHasteMod;
        public float PercentCooldownMod => this.Entity.PercentCooldownMod;
        public float FlatMagicDamageMod => this.Entity.FlatMagicDamageMod;
        public float PercentMagicDamageMod => this.Entity.PercentMagicDamageMod;
        public float BaseAbilityDamage => this.Entity.BaseAbilityDamage;
        public float FlatPhysicalDamageMod => this.Entity.FlatPhysicalDamageMod;
        public float PercentPhysicalDamageMod => this.Entity.PercentPhysicalDamageMod;
        public float PercentBonusPhysicalDamageMod => this.Entity.PercentBonusPhysicalDamageMod;
        public float PercentBasePhysicalDamageAsFlatBonusMod => this.Entity.PercentBasePhysicalDamageAsFlatBonusMod;
        public float BaseAttackDamage => this.Entity.BaseAttackDamage;
        public float PercentLifeStealMod => this.Entity.PercentLifeStealMod;
        public float PercentAttackSpeedMod => this.Entity.PercentAttackSpeedMod;
        public float AttackSpeedMod => this.Entity.AttackSpeedMod;
        public float FlatCritChanceMod => this.Entity.FlatCritChanceMod;
        public float FlatCritDamageMod => this.Entity.FlatCritDamageMod;
        public float PercentCritDamageMod => this.Entity.PercentCritDamageMod;
        public float CritDamageMultiplier => this.Entity.CritDamageMultiplier;
        public float Crit => this.Entity.Crit;
        public float Armor => this.Entity.Armor;
        public float BonusArmor => this.Entity.BonusArmor;
        public float SpellBlock => this.Entity.SpellBlock;
        public float BonusSpellBlock => this.Entity.BonusSpellBlock;
        public float BasePARRegenRate => this.Entity.BasePARRegenRate;
        public float PARRegenRate => this.Entity.PARRegenRate;
        public float BaseHPRegenRate => this.Entity.BaseHPRegenRate;
        public float HPRegenRate => this.Entity.HPRegenRate;
        public float MoveSpeed => this.Entity.MoveSpeed;
        public float AttackRange => this.Entity.AttackRange;
        public float FlatArmorPenetrationMod => this.Entity.FlatArmorPenetrationMod;
        public float FlatMagicPenetrationMod => this.Entity.FlatMagicPenetrationMod;
        public float PercentArmorPenetrationMod => this.Entity.PercentArmorPenetrationMod;
        public float PercentMagicPenetrationMod => this.Entity.PercentMagicPenetrationMod;
        public float PercentBonusArmorPenetrationMod => this.Entity.PercentBonusArmorPenetrationMod;
        public float PercentBonusMagicPenetrationMod => this.Entity.PercentBonusMagicPenetrationMod;
        public float PhysicalLethality => this.Entity.PhysicalLethality;
        public float MagicLethality => this.Entity.MagicLethality;
        public float TotalAttackDamage => this.Entity.TotalAttackDamage;
        public float TotalMagicalDamage => this.Entity.TotalMagicalDamage;
        public float BaseHP => this.Entity.BaseHP;
        public float BaseMoveSpeed => this.Entity.BaseMoveSpeed;
        public float GoldTotal => this.Entity.GoldTotal;
        public float BonusHealth => this.Entity.BonusHealth;
        public float BonusMana => this.Entity.BonusMana;
        public Vector3 Velocity => this.Entity.Velocity;
        public Vector3 InfoComponentBasePosition => this.Entity.InfoComponentBasePosition;
        public ISpellData BasicAttack => EntityConverter.Convert(this.Entity.BasicAttack);
        public IItem[] InventoryItems => Array.ConvertAll(this.Entity.InventoryItems, EntityConverter.Convert);

        public float GetAbilityResource(AbilityResourceType abilityResourceType, OutputType outputType)
        {
            return this.Entity.GetAbilityResource((EnsoulSharp.AbilityResourceType)abilityResourceType, (EnsoulSharp.OutputType)outputType);
        }

        public IBuff GetBuff(string buffName)
        {
            return EntityConverter.Convert(this.Entity.GetBuff(buffName));
        }

        public int GetBuffCount(string buffName)
        {
            return this.Entity.GetBuffCount(buffName);
        }

        public Vector3[] GetPath(Vector3 end, bool smoothPath)
        {
            return this.Entity.GetPath(end, smoothPath);
        }

        public Vector3[] GetPath(Vector3 start, Vector3 end, bool smoothPath)
        {
            return this.Entity.GetPath(start, end, smoothPath);
        }

        public bool HasBuff(string buffName)
        {
            return this.Entity.HasBuff(buffName);
        }

        public bool HasBuffOfType(BuffType buffType)
        {
            return this.Entity.HasBuffOfType((EnsoulSharp.BuffType)buffType);
        }

        public void SetSkin(string model, int id)
        {
            this.Entity.SetSkin(id);
        }

        public bool CanUseObject(IAttackable target)
        {
            return this.Entity.CanUseObject(EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AttackableUnit>(target.Id));
        }

        public bool UseObject(IAttackable target)
        {
            return this.Entity.UseObject(EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AttackableUnit>(target.Id));
        }
    }
}