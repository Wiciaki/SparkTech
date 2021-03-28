/*namespace SparkTech.SDK.SpellDatabase
{
    using SparkTech.SDK.Entities;

    public class PlayerSpell : Spell
    {
        public PlayerSpell(SpellSlot slot) : base(ObjectManager.Player, slot)
        {

        }

        public AIHeroClient SelectTarget(IEnumerable<AIHeroClient> heroes)
        {
            return heroes.Where(this.IsInRange).SelectTarget();
        }

        public AIHeroClient SelectTarget()
        {
            return this.SelectTarget(ObjectManager.Get<AIHeroClient>());
        }
    }
}*/