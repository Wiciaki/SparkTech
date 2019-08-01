namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.Entities.Buffs;

    public interface IAIBase : IAttackableUnit
    {
        string BaseSkinName();

        string CharName();

        bool HasBuffOfType(BuffType buff);

        IBuff GetBuff(string buffName);

        void SetSkin(string model, int id);

        Vector3[] GetPath(Vector3 end);

        Vector3[] GetPath(Vector3 start, Vector3 end);

        bool IsWindingUp();

        bool IsHoldingPosition();

        string SkinName();

        bool PlayerControlled();

        float ExpGiveRadius();

        GameObjectCharacterState CharacterState();

        int LastPetSpawnedId();

        float Gold();

        GameObjectCombatType CombatType();

        float DeathDuration();

        bool IsMoving();

        Vector3[] Path();

        Spellbook Spellbook();
    }
}