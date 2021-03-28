namespace SparkTech.SDK.API
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public interface IPlayerFragment
    {
        int EvolvePoints { get; }

        int SpellTrainingPoints { get; }

        void IssueOrder(GameObjectOrder order, Vector3 target);

        void IssueOrder(GameObjectOrder order, IAttackable target);

        void UpdateChargedSpell(SpellSlot slot, Vector3 target, bool releaseCast);

        bool CastSpell(SpellSlot slot, Vector3 position);

        bool CastSpell(SpellSlot slot, Vector3 startPosition, Vector3 endPosition);

        bool CastSpell(SpellSlot slot, IGameObject target); // why not IAttackable?

        bool CastSpell(SpellSlot slot);

        void LevelSpell(SpellSlot slot);

        void EvolveSpell(SpellSlot slot);

        BuyItemResult BuyItem(ItemId itemId);

        void SellItem(int slot);
    }
}