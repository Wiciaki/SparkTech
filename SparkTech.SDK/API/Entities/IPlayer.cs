namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.Entities.Spells;

    public interface IPlayer : IHero
    {
        void IssueOrder(GameObjectOrder order, Vector3 targetPos);

        void IssueOrder(GameObjectOrder order, IGameObject target);

        bool UpdateChargedSpell(SpellSlot slot, Vector3 position);

        bool CastSpell(SpellCastInput args);

        void LevelSpell(SpellSlot slot);

        void EvolveSpell(SpellSlot slot);
    }
}