namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IPlayer : IHero
    {
        // todo ??

        void IssueOrder(GameObjectOrder order, Vector3 targetPos);

        void IssueOrder(GameObjectOrder order, IGameObject target);

        bool UpdateChargedSpell(SpellSlot slot, Vector3 position);

        //bool CastSpell(SpellCastInput args);

        void LevelSpell(SpellSlot slot);

        void EvolveSpell(SpellSlot slot);
    }
}