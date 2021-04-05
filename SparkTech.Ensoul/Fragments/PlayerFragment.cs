namespace SparkTech.Ensoul.Fragments
{
    using SharpDX;

    using SparkTech.SDK.API;
    using SparkTech.SDK.Entities;

    public class PlayerFragment : IPlayerFragment
    {
        private readonly EnsoulSharp.AIHeroClient player;
        private readonly EnsoulSharp.Spellbook book;

        public PlayerFragment()
        {
            this.player = EnsoulSharp.ObjectManager.Player;
            this.book = this.player.Spellbook;
        }

        public int SpellTrainingPoints => this.player.SpellTrainingPoints;
        public int EvolvePoints => this.player.EvolvePoints;

        public bool IssueOrder(GameObjectOrder order, Vector3 target)
        {
            return this.player.IssueOrder((EnsoulSharp.GameObjectOrder)order, target);
        }

        public bool IssueOrder(GameObjectOrder order, IAttackable target)
        {
            return this.player.IssueOrder((EnsoulSharp.GameObjectOrder)order, EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.AttackableUnit>(target.Id));
        }

        public void LevelSpell(SpellSlot slot)
        {
            this.book.LevelSpell((EnsoulSharp.SpellSlot)slot);
        }

        public void EvolveSpell(SpellSlot slot)
        {
            this.book.EvolveSpell((EnsoulSharp.SpellSlot)slot);
        }

        public bool UpdateChargedSpell(SpellSlot slot, Vector3 target, bool releaseCast)
        {
            return this.book.UpdateChargedSpell((EnsoulSharp.SpellSlot)slot, target, releaseCast);
        }

        public bool CastSpell(SpellSlot slot, Vector3 position)
        {
            return this.book.CastSpell((EnsoulSharp.SpellSlot)slot, position);
        }

        public bool CastSpell(SpellSlot slot, Vector3 startPosition, Vector3 endPosition)
        {
            return this.book.CastSpell((EnsoulSharp.SpellSlot)slot, startPosition, endPosition);
        }

        public bool CastSpell(SpellSlot slot, IGameObject target)
        {
            return this.book.CastSpell((EnsoulSharp.SpellSlot)slot, EnsoulSharp.ObjectManager.GetUnitByNetworkId<EnsoulSharp.GameObject>(target.Id));
        }

        public bool CastSpell(SpellSlot slot)
        {
            return this.book.CastSpell((EnsoulSharp.SpellSlot)slot);
        }

        public BuyItemResult BuyItem(ItemId itemId)
        {
            return (BuyItemResult)this.player.BuyItem((EnsoulSharp.ItemId)itemId);
        }

        public void SellItem(int slot)
        {
            this.player.SellItem(slot);
        }
    }
}