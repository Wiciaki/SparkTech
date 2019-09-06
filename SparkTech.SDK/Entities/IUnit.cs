namespace SparkTech.SDK.Entities
{
    using SharpDX;

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

        //?
        //bool PlayerControlled { get; }
    }
}