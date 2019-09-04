namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.API.Fragments;
    using SparkTech.SDK.Entities.Buffs;

    public interface IUnit : IAttackableUnit
    {
        string BaseSkinName { get; }

        string CharName { get; }

        bool HasBuffOfType(BuffType buff);

        IBuff GetBuff(string buffName);

        int GetBuffCount(string buffName);

        void SetSkin(string model, int id);

        Vector3[] GetPath(Vector3 end);
        Vector3[] GetPath(Vector3 start, Vector3 end);

        bool IsWindingUp { get; }

        bool IsHoldingPosition { get; }

        string SkinName { get; }

        float ExpGiveRadius { get; }

        GameObjectCharacterState CharacterState { get; }

        int LastPetSpawnedId { get; }

        float Gold { get; }

        GameObjectCombatType CombatType { get; }

        float DeathDuration { get; }

        bool IsMoving { get; }

        Vector3[] Path { get; }

        ISpellbook Spellbook { get; }

        bool CanAttack { get; }
        bool CanCast { get; }
        bool CanMove { get; }

        bool IsImmovable { get; }
        bool IsRooted { get; }
        bool IsPacified { get; }
        bool IsCharmed { get; }

        IBuff[] Buffs { get; }
        
        // ISpell BasicAttack { get; }

        // IItem[] Items { get; }
        
        IGameObject Pet { get; }

        Vector3 ServerPosition { get; }

        float AttackDelay { get; }
        float AttackCastDelay { get; }

        Vector2 HealthBarPosition { get; }
        bool IsHealthBarRendered { get; }

        Vector3 Direction { get; }

        int BaseSkinId { get; }

        IAttackableUnit Target { get; }
        // ICharacterData CharData { get; }

        // event Obj_AI_BasePlayAnimation OnPlayAnimation
        // event Obj_AI_BaseProcessSpellCast OnProcessSpellCast
        // event Obj_AI_BaseDoCast OnDoCast
        // event Obj_AI_BaseNewPath OnNewPath
        // event Obj_AI_BaseIssueOrder OnIssueOrder
        // event Obj_AI_BaseTeleport OnTeleport
        // event Obj_AI_BaseAggro OnAggro
        // event Obj_AI_BaseSwapItem OnSwapItem
        // event Obj_AI_BasePlaceItemInSlot OnPlaceItemInSlot
        // event Obj_AI_BaseRemoveItem OnRemoveItem
        // event Obj_AI_BaseBuffAdd OnBuffAdd
        // event Obj_AI_BaseBuffRemove OnBuffRemove
        // event Obj_AI_BaseBuffUpdateCount OnBuffUpdateCount
        // event Obj_AI_BaseLevelUp OnLevelUp
        // event Obj_AI_BasePauseAnimation OnPauseAnimation
        // event Obj_AI_BaseTarget OnTarget


        //?
        //bool PlayerControlled { get; }
    }
}