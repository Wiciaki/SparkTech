namespace Surgical.SDK.API
{
    using SharpDX;

    using Surgical.SDK.Entities;

    public interface IPlayerFragment
    {
        int SpellTrainingPoints { get; }

        void IssueOrder(GameObjectOrder order, Vector3 target);

        void IssueOrder(GameObjectOrder order, IGameObject target);

        void UpdateChargedSpell(SpellSlot slot, Vector3 target);

        //bool CastSpell(SpellCastInput args);

        void LevelSpell(SpellSlot slot);

        void EvolveSpell(SpellSlot slot);
    }
}