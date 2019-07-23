namespace SparkTech.Database
{
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.Caching;
    using SparkTech.TargetSelector;

    public class PlayerSpell : Spell
    {
        public PlayerSpell(SpellSlot slot) : base(ObjectCache.Player, slot)
        {

        }

        public AIHeroClient SelectTarget(IEnumerable<AIHeroClient> heroes)
        {
            return heroes.Where(this.IsInRange).SelectTarget();
        }

        public AIHeroClient SelectTarget()
        {
            return this.SelectTarget(ObjectCache.Get<AIHeroClient>());
        }
    }
}